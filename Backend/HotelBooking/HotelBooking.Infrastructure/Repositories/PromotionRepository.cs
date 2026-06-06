using Microsoft.EntityFrameworkCore;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Interfaces.Repositories;
using HotelBooking.Infrastructure.Data;

namespace HotelBooking.Infrastructure.Repositories
{
    public class PromotionRepository : GenericRepository<Promotion>, IPromotionRepository
    {
        public PromotionRepository(AppDbContext context) : base(context) { }

        public async Task<Promotion> GetByCodeAsync(string code)
            => await _dbSet.FirstOrDefaultAsync(p =>
                p.Code == code &&
                p.IsActive &&
                !p.IsDeleted);

        public async Task<IEnumerable<Promotion>> GetActivePromotionsAsync()
            => await _dbSet
                .Where(p =>
                    p.IsActive &&
                    !p.IsDeleted &&
                    p.ValidFrom <= DateTime.UtcNow &&
                    p.ValidTo >= DateTime.UtcNow)
                .ToListAsync();
    }
}
