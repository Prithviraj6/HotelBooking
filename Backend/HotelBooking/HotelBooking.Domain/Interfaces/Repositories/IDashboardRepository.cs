using HotelBooking.Domain.Enums;

namespace HotelBooking.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Dashboard-specific aggregation queries — uses DB-level COUNT/SUM
    /// instead of loading all rows into memory, making admin stats fast at scale.
    /// </summary>
    public interface IDashboardRepository
    {
        Task<int> GetTotalHotelsAsync();
        Task<int> GetTotalRoomsAsync();
        Task<int> GetTotalUsersAsync();
        Task<int> GetTotalBookingsAsync();
        Task<int> GetBookingsByStatusAsync(BookingStatus status);
        Task<int> GetBookingsThisMonthAsync();
        Task<decimal> GetTotalRevenueAsync();
        Task<decimal> GetRevenueThisMonthAsync();

        /// <summary>Occupancy = (Confirmed + Completed bookings) / total rooms × 100.</summary>
        Task<double> GetOccupancyRateAsync();
    }
}
