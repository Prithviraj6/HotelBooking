using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Interfaces.Repositories;

public interface IRoomTypeRepository : IGenericRepository<RoomType>
{
    Task<IEnumerable<RoomType>> GetByHotelAsync(int hotelId);
}