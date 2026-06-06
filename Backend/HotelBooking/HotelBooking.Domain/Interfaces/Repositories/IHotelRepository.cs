using HotelBooking.Domain.Entities;

namespace HotelBooking.Domain.Interfaces.Repositories
{
    public interface IHotelRepository : IGenericRepository<Hotel>
    {
        Task<IEnumerable<Hotel>> SearchHotelsAsync(string city, int? starRating, string search);
        Task<Hotel> GetHotelWithRoomsAsync(int hotelId);
        Task<Hotel> GetHotelWithReviewsAsync(int hotelId);
    }
}
