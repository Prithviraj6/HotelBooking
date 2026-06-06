using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Interfaces.Repositories;
using HotelBooking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Infrastructure.Repositories
{
    public class HotelRepository : GenericRepository<Hotel>, IHotelRepository
    {
        public HotelRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Hotel>> SearchHotelsAsync(
            string city, int? starRating, string search)
        {
            var query = _dbSet
                .Where(h => !h.IsDeleted && h.IsActive)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(city))
                query = query.Where(h => h.City.ToLower().Contains(city.ToLower()));

            if (starRating.HasValue)
                query = query.Where(h => h.StarRating == starRating.Value);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(h =>
                    h.Name.ToLower().Contains(search.ToLower()) ||
                    h.Location.ToLower().Contains(search.ToLower()));

            return await query.ToListAsync();
        }

        public async Task<Hotel> GetHotelWithRoomsAsync(int hotelId)
            => await _dbSet
                .Include(h => h.Rooms)
                    .ThenInclude(r => r.RoomType)
                .FirstOrDefaultAsync(h => h.Id == hotelId && !h.IsDeleted);

        public async Task<Hotel> GetHotelWithReviewsAsync(int hotelId)
            => await _dbSet
                .Include(h => h.Reviews)
                    .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(h => h.Id == hotelId && !h.IsDeleted);
    }
}
