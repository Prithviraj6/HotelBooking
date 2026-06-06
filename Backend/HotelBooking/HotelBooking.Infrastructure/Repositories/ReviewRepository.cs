using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Interfaces.Repositories;
using HotelBooking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Infrastructure.Repositories
{
    public class ReviewRepository : GenericRepository<Review>, IReviewRepository
    {
        public ReviewRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Review>> GetReviewsByHotelAsync(int hotelId)
            => await _dbSet
                .Include(r => r.User)
                .Where(r => r.HotelId == hotelId && !r.IsDeleted)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

        public async Task<bool> HasUserReviewedBookingAsync(int userId, int bookingId)
            => await _dbSet.AnyAsync(r =>
                r.UserId == userId &&
                r.BookingId == bookingId &&
                !r.IsDeleted);
    }
}
