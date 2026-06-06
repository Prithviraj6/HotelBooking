using HotelBooking.Domain.Entities;

namespace HotelBooking.Domain.Interfaces.Repositories
{
    public interface IPromotionRepository : IGenericRepository<Promotion>
    {
        Task<Promotion> GetByCodeAsync(string code);
        Task<IEnumerable<Promotion>> GetActivePromotionsAsync();
    }
}
