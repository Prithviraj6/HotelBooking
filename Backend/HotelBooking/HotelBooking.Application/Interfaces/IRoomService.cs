using HotelBooking.Application.DTOs.Room;
using HotelBooking.Domain.Enums;

namespace HotelBooking.Application.Interfaces
{
    public interface IRoomService
    {
        Task<IEnumerable<RoomResponseDto>> GetAllAsync();
        Task<RoomResponseDto> GetByIdAsync(int id);
        Task<IEnumerable<RoomResponseDto>> GetAvailableRoomsAsync(
            int hotelId, DateTime checkIn, DateTime checkOut);
        Task<RoomResponseDto> CreateAsync(CreateRoomDto dto);
        Task<RoomResponseDto> UpdateAsync(int id, UpdateRoomDto dto);
        Task DeleteAsync(int id);
        Task UpdateStatusAsync(int id, RoomStatus status);
    }
}
