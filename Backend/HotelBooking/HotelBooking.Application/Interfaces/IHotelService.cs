using HotelBooking.Application.DTOs.Common;
using HotelBooking.Application.DTOs.Hotel;
using HotelBooking.Application.DTOs.Review;
using HotelBooking.Application.DTOs.Room;
using HotelBooking.Application.DTOs.RoomType;
using HotelBooking.Domain.Common;

namespace HotelBooking.Application.Interfaces
{
    public interface IHotelService
    {
        Task<PagedResponse<HotelResponseDto>> SearchHotelsAsync(HotelSearchDto searchDto);
        Task<HotelResponseDto> GetByIdAsync(int id);
        Task<HotelResponseDto> CreateAsync(CreateHotelDto dto);
        Task<HotelResponseDto> UpdateAsync(int id, UpdateHotelDto dto);
        Task DeleteAsync(int id);
        Task<IEnumerable<RoomResponseDto>> GetHotelRoomsAsync(int hotelId);
        Task<IEnumerable<ReviewResponseDto>> GetHotelReviewsAsync(int hotelId);
        Task<IEnumerable<RoomTypeResponseDto>> GetHotelRoomTypesAsync(int hotelId);
    }
}
