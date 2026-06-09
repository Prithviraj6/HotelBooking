using AutoMapper;
using HotelBooking.Application.DTOs.Booking;
using HotelBooking.Application.Interfaces;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Enums;
using HotelBooking.Domain.Interfaces;

namespace HotelBooking.Application.Services
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;

        public BookingService(
            IUnitOfWork unitOfWork,
            IEmailService emailService,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _mapper = mapper;
        }

        public async Task<BookingResponseDto> CreateAsync(int userId, CreateBookingDto dto)
        {
            // Date validation (also covered by FluentValidation at the controller layer)
            if (dto.CheckInDate.Date < DateTime.UtcNow.Date)
                throw new ArgumentException("Check-in date cannot be in the past.");

            if (dto.CheckOutDate.Date <= dto.CheckInDate.Date)
                throw new ArgumentException("Check-out date must be after check-in date.");

            var roomType = await _unitOfWork.RoomTypes.GetByIdAsync(dto.RoomTypeId);
            if (roomType == null)
                throw new KeyNotFoundException($"Room type with ID {dto.RoomTypeId} not found.");

            var availableRooms = await _unitOfWork.Rooms.GetAvailableRoomsAsync(
                roomType.HotelId, dto.CheckInDate, dto.CheckOutDate);

            var room = availableRooms.FirstOrDefault(r => r.RoomTypeId == dto.RoomTypeId);
            if (room == null)
                throw new InvalidOperationException("No rooms of the selected type are available for the selected dates.");

            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("User not found.");

            var totalNights = (dto.CheckOutDate.Date - dto.CheckInDate.Date).Days;
            var totalPrice = totalNights * roomType.PricePerNight;
            var discountAmount = 0m;

            // Apply promo code — normalize to uppercase before lookup (bug fix)
            if (!string.IsNullOrWhiteSpace(dto.PromoCode))
            {
                var normalizedCode = dto.PromoCode.ToUpper().Trim();
                var promotion = await _unitOfWork.Promotions.GetByCodeAsync(normalizedCode);

                if (promotion == null)
                    throw new ArgumentException("Invalid promo code.");

                if (promotion.ValidFrom > DateTime.UtcNow || promotion.ValidTo < DateTime.UtcNow)
                    throw new ArgumentException("Promo code has expired.");

                if (promotion.MaxUsageCount.HasValue &&
                    promotion.UsedCount >= promotion.MaxUsageCount.Value)
                    throw new ArgumentException("Promo code has reached maximum usage.");

                if (promotion.HotelId.HasValue && promotion.HotelId != room.HotelId)
                    throw new ArgumentException("Promo code is not valid for this hotel.");

                discountAmount = totalPrice * (promotion.DiscountPercent / 100);
                promotion.UsedCount++;
                await _unitOfWork.Promotions.UpdateAsync(promotion);
            }

            var hotel = await _unitOfWork.Hotels.GetByIdAsync(room.HotelId);

            var booking = new Booking
            {
                UserId = userId,
                RoomId = room.Id,
                CheckInDate = dto.CheckInDate,
                CheckOutDate = dto.CheckOutDate,
                TotalNights = totalNights,
                TotalPrice = totalPrice - discountAmount,
                DiscountAmount = discountAmount,
                Status = BookingStatus.Pending,
                SpecialRequests = dto.SpecialRequests,
                PromoCode = dto.PromoCode?.ToUpper().Trim(),
                // Attach nav props for AutoMapper
                User = user,
                Room = room
            };
            room.Hotel = hotel;
            room.RoomType = roomType;

            await _unitOfWork.Bookings.AddAsync(booking);
            await _unitOfWork.SaveChangesAsync();

            await _emailService.SendBookingConfirmationAsync(
                user.Email,
                user.FirstName,
                booking.Id,
                hotel?.Name,
                booking.CheckInDate,
                booking.CheckOutDate,
                booking.TotalPrice);

            return _mapper.Map<BookingResponseDto>(booking);
        }

        public async Task<IEnumerable<BookingResponseDto>> GetMyBookingsAsync(int userId)
        {
            // GetBookingsByUserAsync already eager-loads Room→Hotel, Room→RoomType, User, Payment
            var bookings = await _unitOfWork.Bookings.GetBookingsByUserAsync(userId);
            return _mapper.Map<List<BookingResponseDto>>(bookings);
        }

        public async Task<BookingResponseDto> GetByIdAsync(int bookingId, int userId, string role)
        {
            var booking = await _unitOfWork.Bookings.GetBookingWithDetailsAsync(bookingId);
            if (booking == null)
                throw new KeyNotFoundException($"Booking with ID {bookingId} not found.");

            if (role == "Customer" && booking.UserId != userId)
                throw new UnauthorizedAccessException("You are not authorized to view this booking.");

            return _mapper.Map<BookingResponseDto>(booking);
        }

        public async Task<BookingResponseDto> CancelAsync(int bookingId, int userId)
        {
            var booking = await _unitOfWork.Bookings.GetBookingWithDetailsAsync(bookingId);
            if (booking == null)
                throw new KeyNotFoundException($"Booking with ID {bookingId} not found.");

            if (booking.UserId != userId)
                throw new UnauthorizedAccessException("You are not authorized to cancel this booking.");

            if (booking.Status == BookingStatus.Cancelled)
                throw new InvalidOperationException("Booking is already cancelled.");

            if (booking.Status == BookingStatus.Completed)
                throw new InvalidOperationException("Cannot cancel a completed booking.");

            if (booking.CheckInDate <= DateTime.UtcNow.AddHours(24))
                throw new InvalidOperationException(
                    "Cannot cancel booking within 24 hours of check-in.");

            booking.Status = BookingStatus.Cancelled;
            await _unitOfWork.Bookings.UpdateAsync(booking);
            await _unitOfWork.SaveChangesAsync();

            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            await _emailService.SendBookingCancellationAsync(user.Email, user.FirstName, bookingId);

            return _mapper.Map<BookingResponseDto>(booking);
        }

        public async Task<IEnumerable<BookingResponseDto>> GetAllAsync()
        {
            // GetAllBookingsAsync already eager-loads all nav props
            var bookings = await _unitOfWork.Bookings.GetAllBookingsAsync();
            return _mapper.Map<List<BookingResponseDto>>(bookings);
        }

        public async Task<BookingResponseDto> UpdateStatusAsync(int bookingId, BookingStatus status)
        {
            var booking = await _unitOfWork.Bookings.GetBookingWithDetailsAsync(bookingId);
            if (booking == null)
                throw new KeyNotFoundException($"Booking with ID {bookingId} not found.");

            if (booking.Status == BookingStatus.Cancelled)
                throw new InvalidOperationException("Cannot update status of a cancelled booking.");

            if (booking.Status == BookingStatus.Completed)
                throw new InvalidOperationException("Cannot update status of a completed booking.");

            booking.Status = status;
            await _unitOfWork.Bookings.UpdateAsync(booking);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<BookingResponseDto>(booking);
        }

        public async Task<BookingResponseDto> RebookAsync(int bookingId, int userId)
        {
            var original = await _unitOfWork.Bookings.GetBookingWithDetailsAsync(bookingId);
            if (original == null)
                throw new KeyNotFoundException($"Booking with ID {bookingId} not found.");

            if (original.UserId != userId)
                throw new UnauthorizedAccessException("You are not authorized to rebook this booking.");

            if (original.Status != BookingStatus.Completed &&
                original.Status != BookingStatus.Cancelled)
                throw new InvalidOperationException(
                    "Can only rebook from completed or cancelled bookings.");

            var room = await _unitOfWork.Rooms.GetByIdAsync(original.RoomId);
            if (room == null)
                throw new KeyNotFoundException("Original room no longer exists.");

            var nights = original.TotalNights;
            var newCheckIn = DateTime.UtcNow.Date.AddDays(1);
            var newCheckOut = newCheckIn.AddDays(nights);

            var isAvailable = await _unitOfWork.Rooms.IsRoomAvailableAsync(
                room.Id, newCheckIn, newCheckOut);

            if (!isAvailable)
                throw new InvalidOperationException(
                    "Room is not available for rebooking. Please choose different dates.");

            var createDto = new CreateBookingDto
            {
                RoomTypeId = room.RoomTypeId, // Use the original room's RoomTypeId
                CheckInDate = newCheckIn,
                CheckOutDate = newCheckOut,
                SpecialRequests = original.SpecialRequests
            };

            return await CreateAsync(userId, createDto);
        }
    }
}