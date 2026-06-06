using HotelBooking.Application.DTOs.Review;

namespace HotelBooking.Application.Interfaces
{
    public interface IReviewService
    {
        Task<ReviewResponseDto> CreateAsync(int userId, CreateReviewDto dto);
        Task<IEnumerable<ReviewResponseDto>> GetByHotelAsync(int hotelId);
        Task<ReviewResponseDto> UpdateAsync(int reviewId, int userId, UpdateReviewDto dto);
        Task DeleteAsync(int reviewId, int userId, string role);
    }
}
