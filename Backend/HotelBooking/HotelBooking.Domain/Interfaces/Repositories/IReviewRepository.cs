using HotelBooking.Domain.Entities;

namespace HotelBooking.Domain.Interfaces.Repositories
{
    public interface IReviewRepository : IGenericRepository<Review>
    {
        Task<IEnumerable<Review>> GetReviewsByHotelAsync(int hotelId);
        Task<bool> HasUserReviewedBookingAsync(int userId, int bookingId);
    }
}
