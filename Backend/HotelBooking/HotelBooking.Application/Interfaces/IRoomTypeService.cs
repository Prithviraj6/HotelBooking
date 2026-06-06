using HotelBooking.Application.DTOs.RoomType;

namespace HotelBooking.Application.Interfaces
{
    public interface IRoomTypeService
    {
        Task<IEnumerable<RoomTypeResponseDto>> GetByHotelAsync(int hotelId);
        Task<RoomTypeResponseDto> GetByIdAsync(int id);
        Task<RoomTypeResponseDto> CreateAsync(CreateRoomTypeDto dto);
        Task<RoomTypeResponseDto> UpdateAsync(int id, UpdateRoomTypeDto dto);
        Task DeleteAsync(int id);
    }
}
