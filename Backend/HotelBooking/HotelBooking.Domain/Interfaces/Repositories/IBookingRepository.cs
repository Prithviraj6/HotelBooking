using HotelBooking.Domain.Entities;

namespace HotelBooking.Domain.Interfaces.Repositories
{
    public interface IBookingRepository : IGenericRepository<Booking>
    {
        Task<IEnumerable<Booking>> GetBookingsByUserAsync(int userId);
        Task<IEnumerable<Booking>> GetAllBookingsAsync();
        Task<Booking> GetBookingWithDetailsAsync(int bookingId);
        Task<bool> HasUserCompletedBookingAsync(int userId, int hotelId);
    }
}
