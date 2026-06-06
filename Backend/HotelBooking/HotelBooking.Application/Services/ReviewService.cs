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

        public ReviewService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ReviewResponseDto> CreateAsync(int userId, CreateReviewDto dto)
        {
            // Check hotel exists
            var hotel = await _unitOfWork.Hotels.GetByIdAsync(dto.HotelId);
            if (hotel == null)
                throw new KeyNotFoundException(
                    $"Hotel with ID {dto.HotelId} not found.");

            // Check booking exists
            var booking = await _unitOfWork.Bookings
                .GetBookingWithDetailsAsync(dto.BookingId);

            if (booking == null)
                throw new KeyNotFoundException(
                    $"Booking with ID {dto.BookingId} not found.");

            // Check booking belongs to user
            if (booking.UserId != userId)
                throw new UnauthorizedAccessException(
                    "You can only review your own bookings.");

            // Only completed bookings can be reviewed
            if (booking.Status != BookingStatus.Completed)
                throw new InvalidOperationException(
                    "You can only review a completed booking.");

            // Check booking is for the same hotel
            var room = booking.Room
                ?? await _unitOfWork.Rooms.GetByIdAsync(booking.RoomId);

            if (room?.HotelId != dto.HotelId)
                throw new ArgumentException(
                    "Booking does not belong to the specified hotel.");

            // Check user has not already reviewed this booking
            var alreadyReviewed = await _unitOfWork.Reviews
                .HasUserReviewedBookingAsync(userId, dto.BookingId);

            if (alreadyReviewed)
                throw new InvalidOperationException(
                    "You have already reviewed this booking.");

            // Validate rating
            if (dto.Rating < 1 || dto.Rating > 5)
                throw new ArgumentException("Rating must be between 1 and 5.");

            var review = new Review
            {
                HotelId = dto.HotelId,
                UserId = userId,
                BookingId = dto.BookingId,
                Rating = dto.Rating,
                Comment = dto.Comment
            };

            await _unitOfWork.Reviews.AddAsync(review);
            await _unitOfWork.SaveChangesAsync();

            var user = await _unitOfWork.Users.GetByIdAsync(userId);

            return MapToResponseDto(review, hotel.Name,
                $"{user?.FirstName} {user?.LastName}");
        }

        public async Task<IEnumerable<ReviewResponseDto>> GetByHotelAsync(int hotelId)
        {
            // Check hotel exists
            var hotel = await _unitOfWork.Hotels.GetByIdAsync(hotelId);
            if (hotel == null)
                throw new KeyNotFoundException(
                    $"Hotel with ID {hotelId} not found.");

            var reviews = await _unitOfWork.Reviews.GetReviewsByHotelAsync(hotelId);

            return reviews.Select(r => MapToResponseDto(
                r,
                hotel.Name,
                $"{r.User?.FirstName} {r.User?.LastName}")).ToList();
        }

        public async Task<ReviewResponseDto> UpdateAsync(
            int reviewId, int userId, UpdateReviewDto dto)
        {
            var review = await _unitOfWork.Reviews.GetByIdAsync(reviewId);
            if (review == null)
                throw new KeyNotFoundException(
                    $"Review with ID {reviewId} not found.");

            // Check ownership
            if (review.UserId != userId)
                throw new UnauthorizedAccessException(
                    "You can only update your own reviews.");

            // Validate rating if provided
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

            var hotel = await _unitOfWork.Hotels.GetByIdAsync(review.HotelId);
            var user = await _unitOfWork.Users.GetByIdAsync(userId);

            return MapToResponseDto(review, hotel?.Name,
                $"{user?.FirstName} {user?.LastName}");
        }

        public async Task DeleteAsync(int reviewId, int userId, string role)
        {
            var review = await _unitOfWork.Reviews.GetByIdAsync(reviewId);
            if (review == null)
                throw new KeyNotFoundException(
                    $"Review with ID {reviewId} not found.");

            // Customer can only delete own review
            // Admin can delete any review
            if (role == "Customer" && review.UserId != userId)
                throw new UnauthorizedAccessException(
                    "You can only delete your own reviews.");

            await _unitOfWork.Reviews.DeleteAsync(review);
            await _unitOfWork.SaveChangesAsync();
        }

        // ─── Private Mapper ──────────────────────────────────────────────

        private static ReviewResponseDto MapToResponseDto(
            Review review, string hotelName, string userName)
        {
            return new ReviewResponseDto
            {
                Id = review.Id,
                HotelId = review.HotelId,
                HotelName = hotelName,
                UserId = review.UserId,
                UserName = userName,
                BookingId = review.BookingId,
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = review.CreatedAt
            };
        }
    }
}