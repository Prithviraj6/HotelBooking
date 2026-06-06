using Microsoft.EntityFrameworkCore;
using HotelBooking.Domain.Entities;
using HotelBooking.Infrastructure.Data;

namespace HotelBooking.Infrastructure.Repositories
{
    public class RoomTypeRepository : GenericRepository<RoomType>, IRoomTypeRepository
    {
        public RoomTypeRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<RoomType>> GetByHotelAsync(int hotelId)
            => await _dbSet
                .Where(rt => rt.HotelId == hotelId && !rt.IsDeleted)
                .ToListAsync();
    }
}
