using HotelBooking.Domain.Common;
using HotelBooking.Domain.Entities;

namespace HotelBooking.Domain.Interfaces.Repositories
{
    public interface IHotelRepository : IGenericRepository<Hotel>
    {
        Task<(IEnumerable<Hotel> Hotels, int TotalCount)> SearchHotelsAsync(HotelSearchDto searchDto);
        Task<Hotel> GetHotelWithRoomsAsync(int hotelId);
        Task<Hotel> GetHotelWithReviewsAsync(int hotelId);
    }
}
