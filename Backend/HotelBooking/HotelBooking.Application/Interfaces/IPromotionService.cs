using HotelBooking.Application.DTOs.Promotion;

namespace HotelBooking.Application.Interfaces
{
    public interface IPromotionService
    {
        Task<PromotionResponseDto> ValidateAsync(string code, int? hotelId);
        Task<PromotionResponseDto> CreateAsync(CreatePromotionDto dto);
        Task<IEnumerable<PromotionResponseDto>> GetAllAsync();
        Task<PromotionResponseDto> UpdateAsync(int id, CreatePromotionDto dto);
        Task DeleteAsync(int id);
    }
}
