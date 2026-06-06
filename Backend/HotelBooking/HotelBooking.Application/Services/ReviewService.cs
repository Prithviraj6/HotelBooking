using AutoMapper;
using HotelBooking.Application.DTOs.Review;
using HotelBooking.Application.Interfaces;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Enums;
using HotelBooking.Domain.Interfaces;

namespace HotelBooking.Application.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ReviewService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ReviewResponseDto> CreateAsync(int userId, CreateReviewDto dto)
        {
            var hotel = await _unitOfWork.Hotels.GetByIdAsync(dto.HotelId);
            if (hotel == null)
                throw new KeyNotFoundException($"Hotel with ID {dto.HotelId} not found.");

            var booking = await _unitOfWork.Bookings.GetBookingWithDetailsAsync(dto.BookingId);
            if (booking == null)
                throw new KeyNotFoundException($"Booking with ID {dto.BookingId} not found.");

            if (booking.UserId != userId)
                throw new UnauthorizedAccessException("You can only review your own bookings.");

            if (booking.Status != BookingStatus.Completed)
                throw new InvalidOperationException("You can only review a completed booking.");

            var room = booking.Room ?? await _unitOfWork.Rooms.GetByIdAsync(booking.RoomId);
            if (room?.HotelId != dto.HotelId)
                throw new ArgumentException("Booking does not belong to the specified hotel.");

            var alreadyReviewed = await _unitOfWork.Reviews
                .HasUserReviewedBookingAsync(userId, dto.BookingId);
            if (alreadyReviewed)
                throw new InvalidOperationException("You have already reviewed this booking.");

            if (dto.Rating < 1 || dto.Rating > 5)
                throw new ArgumentException("Rating must be between 1 and 5.");

            var user = await _unitOfWork.Users.GetByIdAsync(userId);

            var review = new Review
            {
                HotelId = dto.HotelId,
                UserId = userId,
                BookingId = dto.BookingId,
                Rating = dto.Rating,
                Comment = dto.Comment,
                Hotel = hotel,
                User = user
            };

            await _unitOfWork.Reviews.AddAsync(review);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ReviewResponseDto>(review);
        }

        public async Task<IEnumerable<ReviewResponseDto>> GetByHotelAsync(int hotelId)
        {
            var hotel = await _unitOfWork.Hotels.GetByIdAsync(hotelId);
            if (hotel == null)
                throw new KeyNotFoundException($"Hotel with ID {hotelId} not found.");

            var reviews = await _unitOfWork.Reviews.GetReviewsByHotelAsync(hotelId);

            // Attach hotel nav prop so AutoMapper can resolve HotelName
            foreach (var r in reviews) r.Hotel = hotel;

            return _mapper.Map<List<ReviewResponseDto>>(reviews);
        }

        public async Task<ReviewResponseDto> UpdateAsync(int reviewId, int userId, UpdateReviewDto dto)
        {
            var review = await _unitOfWork.Reviews.GetByIdAsync(reviewId);
            if (review == null)
                throw new KeyNotFoundException($"Review with ID {reviewId} not found.");

            if (review.UserId != userId)
                throw new UnauthorizedAccessException("You can only update your own reviews.");

            if (dto.Rating.HasValue)
            {
                if (dto.Rating < 1 || dto.Rating > 5)
                    throw new ArgumentException("Rating must be between 1 and 5.");
                review.Rating = dto.Rating.Value;
            }

            if (!string.IsNullOrWhiteSpace(dto.Comment))
                review.Comment = dto.Comment;

            await _unitOfWork.Reviews.UpdateAsync(review);
            await _unitOfWork.SaveChangesAsync();

            review.Hotel = await _unitOfWork.Hotels.GetByIdAsync(review.HotelId);
            review.User = await _unitOfWork.Users.GetByIdAsync(userId);

            return _mapper.Map<ReviewResponseDto>(review);
        }

        public async Task DeleteAsync(int reviewId, int userId, string role)
        {
            var review = await _unitOfWork.Reviews.GetByIdAsync(reviewId);
            if (review == null)
                throw new KeyNotFoundException($"Review with ID {reviewId} not found.");

            if (role == "Customer" && review.UserId != userId)
                throw new UnauthorizedAccessException("You can only delete your own reviews.");

            await _unitOfWork.Reviews.DeleteAsync(review);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}