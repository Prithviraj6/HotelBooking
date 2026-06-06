using HotelBooking.Domain.Entities;

namespace HotelBooking.Domain.Interfaces.Repositories
{
    public interface IRoomRepository : IGenericRepository<Room>
    {
        Task<IEnumerable<Room>> GetRoomsByHotelAsync(int hotelId);
        Task<IEnumerable<Room>> GetAvailableRoomsAsync(int hotelId, DateTime checkIn, DateTime checkOut);
        Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkIn, DateTime checkOut);

        /// <summary>Loads all rooms with Hotel and RoomType eagerly — avoids N+1 in listing endpoints.</summary>
        Task<IEnumerable<Room>> GetAllRoomsWithDetailsAsync();
    }
}
