using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Enums;
using HotelBooking.Domain.Interfaces.Repositories;
using HotelBooking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Infrastructure.Repositories
{
    /// <summary>
    /// Uses EF Core DB-level aggregations (CountAsync, SumAsync) — zero in-memory table scans.
    /// </summary>
    public class DashboardRepository : IDashboardRepository
    {
        private readonly AppDbContext _context;

        public DashboardRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<int> GetTotalHotelsAsync()
            => _context.Hotels.CountAsync(h => !h.IsDeleted);

        public Task<int> GetTotalRoomsAsync()
            => _context.Rooms.CountAsync(r => !r.IsDeleted);

        public Task<int> GetTotalUsersAsync()
            => _context.Users.CountAsync(u => !u.IsDeleted);

        public Task<int> GetTotalBookingsAsync()
            => _context.Bookings.CountAsync(b => !b.IsDeleted);

        public Task<int> GetBookingsByStatusAsync(BookingStatus status)
            => _context.Bookings.CountAsync(b => !b.IsDeleted && b.Status == status);

        public Task<int> GetBookingsThisMonthAsync()
        {
            var startOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            return _context.Bookings.CountAsync(b => !b.IsDeleted && b.CreatedAt >= startOfMonth);
        }

        public async Task<decimal> GetTotalRevenueAsync()
            => await _context.Payments
                .Where(p => !p.IsDeleted && p.Status == PaymentStatus.Success)
                .SumAsync(p => (decimal?)p.Amount) ?? 0m;

        public async Task<decimal> GetRevenueThisMonthAsync()
        {
            var startOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            return await _context.Payments
                .Where(p => !p.IsDeleted &&
                            p.Status == PaymentStatus.Success &&
                            p.PaidAt >= startOfMonth)
                .SumAsync(p => (decimal?)p.Amount) ?? 0m;
        }

        public async Task<double> GetOccupancyRateAsync()
        {
            var totalRooms = await GetTotalRoomsAsync();
            if (totalRooms == 0) return 0;

            var activeBookings = await _context.Bookings.CountAsync(b =>
                !b.IsDeleted &&
                (b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Completed));

            return Math.Round((double)activeBookings / totalRooms * 100, 1);
        }
    }
}
