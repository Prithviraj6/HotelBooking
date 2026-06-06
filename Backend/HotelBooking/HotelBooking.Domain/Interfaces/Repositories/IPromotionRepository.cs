using HotelBooking.Domain.Entities;

namespace HotelBooking.Domain.Interfaces.Repositories
{
    public interface IPromotionRepository : IGenericRepository<Promotion>
    {
        Task<Promotion> GetByCodeAsync(string code);
        Task<IEnumerable<Promotion>> GetActivePromotionsAsync();

        /// <summary>Loads all promotions with their Hotel — avoids N+1 in admin listing.</summary>
        Task<IEnumerable<Promotion>> GetAllWithHotelAsync();
    }
}
