using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Enums;
using HotelBooking.Domain.Interfaces.Repositories;
using HotelBooking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Infrastructure.Repositories
{
    public class BookingRepository : GenericRepository<Booking>, IBookingRepository
    {
        public BookingRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Booking>> GetBookingsByUserAsync(int userId)
            => await _dbSet
                .Include(b => b.Room)
                    .ThenInclude(r => r.Hotel)
                .Include(b => b.Room)
                    .ThenInclude(r => r.RoomType)
                .Include(b => b.Payment)
                .Where(b => b.UserId == userId && !b.IsDeleted)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

        public async Task<IEnumerable<Booking>> GetAllBookingsAsync()
            => await _dbSet
                .Include(b => b.User)
                .Include(b => b.Room)
                    .ThenInclude(r => r.Hotel)
                .Include(b => b.Payment)
                .Where(b => !b.IsDeleted)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

        public async Task<Booking> GetBookingWithDetailsAsync(int bookingId)
            => await _dbSet
                .Include(b => b.User)
                .Include(b => b.Room)
                    .ThenInclude(r => r.Hotel)
                .Include(b => b.Room)
                    .ThenInclude(r => r.RoomType)
                .Include(b => b.Payment)
                .Include(b => b.Review)
                .FirstOrDefaultAsync(b => b.Id == bookingId && !b.IsDeleted);

        public async Task<bool> HasUserCompletedBookingAsync(int userId, int hotelId)
            => await _dbSet.AnyAsync(b =>
                b.UserId == userId &&
                b.Room.HotelId == hotelId &&
                b.Status == BookingStatus.Completed &&
                !b.IsDeleted);
    }
}
