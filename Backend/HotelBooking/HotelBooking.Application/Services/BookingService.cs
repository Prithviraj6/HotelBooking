using HotelBooking.Application.Interfaces;
using HotelBooking.Application.DTOs.Booking;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Enums;
using HotelBooking.Domain.Interfaces;

namespace HotelBooking.Application.Services
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;

        public BookingService(
            IUnitOfWork unitOfWork,
            IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
        }

        public async Task<BookingResponseDto> CreateAsync(int userId, CreateBookingDto dto)
        {
            // Validate dates
            if (dto.CheckInDate.Date < DateTime.UtcNow.Date)
                throw new ArgumentException("Check-in date cannot be in the past.");

            if (dto.CheckOutDate.Date <= dto.CheckInDate.Date)
                throw new ArgumentException(
                    "Check-out date must be after check-in date.");

            // Check room exists
            var room = await _unitOfWork.Rooms.GetByIdAsync(dto.RoomId);
            if (room == null)
                throw new KeyNotFoundException($"Room with ID {dto.RoomId} not found.");

            // Check room is not under maintenance or inactive
            if (room.Status == RoomStatus.UnderMaintenance)
                throw new InvalidOperationException(
                    "Room is currently under maintenance.");

            if (room.Status == RoomStatus.InActive)
                throw new InvalidOperationException("Room is not available.");

            // Check room availability for requested dates
            var isAvailable = await _unitOfWork.Rooms.IsRoomAvailableAsync(
                dto.RoomId, dto.CheckInDate, dto.CheckOutDate);

            if (!isAvailable)
                throw new InvalidOperationException(
                    "Room is not available for the selected dates.");

            // Check user exists
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("User not found.");

            // Get room type for pricing
            var roomType = await _unitOfWork.RoomTypes.GetByIdAsync(room.RoomTypeId);
            if (roomType == null)
                throw new KeyNotFoundException("Room type not found.");

            // Calculate total nights and price
            var totalNights = (dto.CheckOutDate.Date - dto.CheckInDate.Date).Days;
            var totalPrice = totalNights * roomType.PricePerNight;
            var discountAmount = 0m;

            // Apply promo code if provided
            if (!string.IsNullOrWhiteSpace(dto.PromoCode))
            {
                var promotion = await _unitOfWork.Promotions
                    .GetByCodeAsync(dto.PromoCode);

                if (promotion == null)
                    throw new ArgumentException("Invalid promo code.");

                if (promotion.ValidFrom > DateTime.UtcNow ||
                    promotion.ValidTo < DateTime.UtcNow)
                    throw new ArgumentException("Promo code has expired.");

                if (promotion.MaxUsageCount.HasValue &&
                    promotion.UsedCount >= promotion.MaxUsageCount.Value)
                    throw new ArgumentException("Promo code has reached maximum usage.");

                // Check if promo is for specific hotel
                if (promotion.HotelId.HasValue &&
                    promotion.HotelId != room.HotelId)
                    throw new ArgumentException(
                        "Promo code is not valid for this hotel.");

                discountAmount = totalPrice * (promotion.DiscountPercent / 100);

                // Increment promo usage count
                promotion.UsedCount++;
                await _unitOfWork.Promotions.UpdateAsync(promotion);
            }

            // Create booking
            var booking = new Booking
            {
                UserId = userId,
                RoomId = dto.RoomId,
                CheckInDate = dto.CheckInDate,
                CheckOutDate = dto.CheckOutDate,
                TotalNights = totalNights,
                TotalPrice = totalPrice - discountAmount,
                DiscountAmount = discountAmount,
                Status = BookingStatus.Pending,
                SpecialRequests = dto.SpecialRequests,
                PromoCode = dto.PromoCode
            };

            await _unitOfWork.Bookings.AddAsync(booking);
            await _unitOfWork.SaveChangesAsync();

            // Send confirmation email
            var hotel = await _unitOfWork.Hotels.GetByIdAsync(room.HotelId);
            await _emailService.SendBookingConfirmationAsync(
                user.Email,
                user.FirstName,
                booking.Id,
                hotel?.Name,
                booking.CheckInDate,
                booking.CheckOutDate,
                booking.TotalPrice);

            return await MapToResponseDtoAsync(booking);
        }

        public async Task<IEnumerable<BookingResponseDto>> GetMyBookingsAsync(int userId)
        {
            var bookings = await _unitOfWork.Bookings.GetBookingsByUserAsync(userId);
            var result = new List<BookingResponseDto>();

            foreach (var booking in bookings)
                result.Add(await MapToResponseDtoAsync(booking));

            return result;
        }

        public async Task<BookingResponseDto> GetByIdAsync(
            int bookingId, int userId, string role)
        {
            var booking = await _unitOfWork.Bookings
                .GetBookingWithDetailsAsync(bookingId);

            if (booking == null)
                throw new KeyNotFoundException(
                    $"Booking with ID {bookingId} not found.");

            // Customer can only see their own bookings
            if (role == "Customer" && booking.UserId != userId)
                throw new UnauthorizedAccessException(
                    "You are not authorized to view this booking.");

            return await MapToResponseDtoAsync(booking);
        }

        public async Task<BookingResponseDto> CancelAsync(int bookingId, int userId)
        {
            var booking = await _unitOfWork.Bookings
                .GetBookingWithDetailsAsync(bookingId);

            if (booking == null)
                throw new KeyNotFoundException(
                    $"Booking with ID {bookingId} not found.");

            // Check ownership
            if (booking.UserId != userId)
                throw new UnauthorizedAccessException(
                    "You are not authorized to cancel this booking.");

            // Can only cancel Pending or Confirmed bookings
            if (booking.Status == BookingStatus.Cancelled)
                throw new InvalidOperationException("Booking is already cancelled.");

            if (booking.Status == BookingStatus.Completed)
                throw new InvalidOperationException(
                    "Cannot cancel a completed booking.");

            // Cannot cancel within 24 hours of check-in
            if (booking.CheckInDate <= DateTime.UtcNow.AddHours(24))
                throw new InvalidOperationException(
                    "Cannot cancel booking within 24 hours of check-in.");

            booking.Status = BookingStatus.Cancelled;

            await _unitOfWork.Bookings.UpdateAsync(booking);
            await _unitOfWork.SaveChangesAsync();

            // Send cancellation email
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            await _emailService.SendBookingCancellationAsync(
                user.Email, user.FirstName, bookingId);

            return await MapToResponseDtoAsync(booking);
        }

        public async Task<IEnumerable<BookingResponseDto>> GetAllAsync()
        {
            var bookings = await _unitOfWork.Bookings.GetAllBookingsAsync();
            var result = new List<BookingResponseDto>();

            foreach (var booking in bookings)
                result.Add(await MapToResponseDtoAsync(booking));

            return result;
        }

        public async Task<BookingResponseDto> UpdateStatusAsync(
            int bookingId, BookingStatus status)
        {
            var booking = await _unitOfWork.Bookings
                .GetBookingWithDetailsAsync(bookingId);

            if (booking == null)
                throw new KeyNotFoundException(
                    $"Booking with ID {bookingId} not found.");

            // Validate status transitions
            if (booking.Status == BookingStatus.Cancelled)
                throw new InvalidOperationException(
                    "Cannot update status of a cancelled booking.");

            if (booking.Status == BookingStatus.Completed)
                throw new InvalidOperationException(
                    "Cannot update status of a completed booking.");

            booking.Status = status;

            await _unitOfWork.Bookings.UpdateAsync(booking);
            await _unitOfWork.SaveChangesAsync();

            return await MapToResponseDtoAsync(booking);
        }

        public async Task<BookingResponseDto> RebookAsync(int bookingId, int userId)
        {
            // Get original booking
            var original = await _unitOfWork.Bookings
                .GetBookingWithDetailsAsync(bookingId);

            if (original == null)
                throw new KeyNotFoundException(
                    $"Booking with ID {bookingId} not found.");

            // Check ownership
            if (original.UserId != userId)
                throw new UnauthorizedAccessException(
                    "You are not authorized to rebook this booking.");

            // Only rebook from completed or cancelled bookings
            if (original.Status != BookingStatus.Completed &&
                original.Status != BookingStatus.Cancelled)
                throw new InvalidOperationException(
                    "Can only rebook from completed or cancelled bookings.");

            // Check room still exists and is available
            var room = await _unitOfWork.Rooms.GetByIdAsync(original.RoomId);
            if (room == null)
                throw new KeyNotFoundException("Original room no longer exists.");

            // New dates — same duration starting from tomorrow
            var nights = original.TotalNights;
            var newCheckIn = DateTime.UtcNow.Date.AddDays(1);
            var newCheckOut = newCheckIn.AddDays(nights);

            // Check availability for new dates
            var isAvailable = await _unitOfWork.Rooms.IsRoomAvailableAsync(
                room.Id, newCheckIn, newCheckOut);

            if (!isAvailable)
                throw new InvalidOperationException(
                    "Room is not available for rebooking. Please choose different dates.");

            var createDto = new CreateBookingDto
            {
                RoomId = original.RoomId,
                CheckInDate = newCheckIn,
                CheckOutDate = newCheckOut,
                SpecialRequests = original.SpecialRequests
            };

            return await CreateAsync(userId, createDto);
        }

        // ─── Private Mapper ──────────────────────────────────────────────

        private async Task<BookingResponseDto> MapToResponseDtoAsync(Booking booking)
        {
            // Load related data if not already loaded
            var room = booking.Room
                ?? await _unitOfWork.Rooms.GetByIdAsync(booking.RoomId);

            var roomType = room?.RoomType
                ?? await _unitOfWork.RoomTypes.GetByIdAsync(room?.RoomTypeId ?? 0);

            var hotel = room?.Hotel
                ?? await _unitOfWork.Hotels.GetByIdAsync(room?.HotelId ?? 0);

            var user = booking.User
                ?? await _unitOfWork.Users.GetByIdAsync(booking.UserId);

            return new BookingResponseDto
            {
                Id = booking.Id,
                UserId = booking.UserId,
                UserName = $"{user?.FirstName} {user?.LastName}",
                UserEmail = user?.Email,
                RoomId = booking.RoomId,
                RoomNumber = room?.RoomNumber,
                RoomType = roomType?.TypeName,
                HotelName = hotel?.Name,
                HotelCity = hotel?.City,
                CheckInDate = booking.CheckInDate,
                CheckOutDate = booking.CheckOutDate,
                TotalNights = booking.TotalNights,
                TotalPrice = booking.TotalPrice,
                DiscountAmount = booking.DiscountAmount,
                PromoCode = booking.PromoCode,
                Status = booking.Status.ToString(),
                SpecialRequests = booking.SpecialRequests,
                CreatedAt = booking.CreatedAt
            };
        }
    }
}