using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Enums;
using HotelBooking.Domain.Interfaces.Repositories;
using HotelBooking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Infrastructure.Repositories
{
    public class RoomRepository : GenericRepository<Room>, IRoomRepository
    {
        public RoomRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Room>> GetRoomsByHotelAsync(int hotelId)
            => await _dbSet
                .Include(r => r.RoomType)
                .Where(r => r.HotelId == hotelId && !r.IsDeleted)
                .ToListAsync();

        public async Task<IEnumerable<Room>> GetAvailableRoomsAsync(
            int hotelId, DateTime checkIn, DateTime checkOut)
            => await _dbSet
                .Include(r => r.RoomType)
                .Where(r =>
                    r.HotelId == hotelId &&
                    !r.IsDeleted &&
                    r.Status == RoomStatus.Available &&
                    !r.Bookings.Any(b =>
                        !b.IsDeleted &&
                        b.Status != BookingStatus.Cancelled &&
                        b.CheckInDate < checkOut &&
                        b.CheckOutDate > checkIn))
                .ToListAsync();

        public async Task<bool> IsRoomAvailableAsync(
            int roomId, DateTime checkIn, DateTime checkOut)
            => !await _context.Bookings.AnyAsync(b =>
                b.RoomId == roomId &&
                !b.IsDeleted &&
                b.Status != BookingStatus.Cancelled &&
                b.CheckInDate < checkOut &&
                b.CheckOutDate > checkIn);

        public async Task<IEnumerable<Room>> GetAllRoomsWithDetailsAsync()
            => await _dbSet
                .Include(r => r.Hotel)
                .Include(r => r.RoomType)
                .Where(r => !r.IsDeleted)
                .OrderBy(r => r.HotelId)
                .ThenBy(r => r.RoomNumber)
                .ToListAsync();
    }
}
