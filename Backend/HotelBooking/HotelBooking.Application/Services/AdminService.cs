using HotelBooking.Application.DTOs.Common;
using HotelBooking.Application.Interfaces;
using HotelBooking.Domain.Enums;
using HotelBooking.Domain.Interfaces;

namespace HotelBooking.Application.Services
{
    public class AdminService : IAdminService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AdminService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DashboardDto> GetDashboardAsync()
        {
            var hotels = await _unitOfWork.Hotels.GetAllAsync();
            var rooms = await _unitOfWork.Rooms.GetAllAsync();
            var users = await _unitOfWork.Users.GetAllAsync();
            var bookings = await _unitOfWork.Bookings.GetAllBookingsAsync();
            var payments = await _unitOfWork.Payments.GetAllPaymentsAsync();

            var allBookings = bookings.ToList();
            var allPayments = payments.ToList();

            var now = DateTime.UtcNow;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);

            var successfulPayments = allPayments
                .Where(p => p.Status == PaymentStatus.Success)
                .ToList();

            var totalRevenue = successfulPayments.Sum(p => p.Amount);

            var revenueThisMonth = successfulPayments
                .Where(p => p.PaidAt >= startOfMonth)
                .Sum(p => p.Amount);

            var bookingsThisMonth = allBookings
                .Count(b => b.CreatedAt >= startOfMonth);

            // Occupancy rate — confirmed + completed bookings / total rooms
            var activeBookings = allBookings
                .Count(b => b.Status == BookingStatus.Confirmed ||
                            b.Status == BookingStatus.Completed);

            var totalRooms = rooms.Count();
            var occupancyRate = totalRooms > 0
                ? Math.Round((double)activeBookings / totalRooms * 100, 1)
                : 0;

            return new DashboardDto
            {
                TotalHotels = hotels.Count(),
                TotalRooms = totalRooms,
                TotalUsers = users.Count(),
                TotalBookings = allBookings.Count,
                PendingBookings = allBookings
                    .Count(b => b.Status == BookingStatus.Pending),
                ConfirmedBookings = allBookings
                    .Count(b => b.Status == BookingStatus.Confirmed),
                CompletedBookings = allBookings
                    .Count(b => b.Status == BookingStatus.Completed),
                CancelledBookings = allBookings
                    .Count(b => b.Status == BookingStatus.Cancelled),
                TotalRevenue = totalRevenue,
                RevenueThisMonth = revenueThisMonth,
                BookingsThisMonth = bookingsThisMonth,
                AverageOccupancyRate = occupancyRate
            };
        }

        public async Task<IEnumerable<BookingReportDto>> GetBookingReportAsync(
            DateTime fromDate, DateTime toDate)
        {
            if (toDate < fromDate)
                throw new ArgumentException(
                    "To date must be after from date.");

            var bookings = await _unitOfWork.Bookings.GetAllBookingsAsync();

            return bookings
                .Where(b => b.CreatedAt.Date >= fromDate.Date &&
                            b.CreatedAt.Date <= toDate.Date)
                .Select(b => new BookingReportDto
                {
                    BookingId = b.Id,
                    UserName = $"{b.User?.FirstName} {b.User?.LastName}",
                    HotelName = b.Room?.Hotel?.Name,
                    RoomNumber = b.Room?.RoomNumber,
                    CheckInDate = b.CheckInDate,
                    CheckOutDate = b.CheckOutDate,
                    TotalNights = b.TotalNights,
                    TotalPrice = b.TotalPrice,
                    Status = b.Status.ToString(),
                    PaymentStatus = b.Payment?.Status.ToString() ?? "Not Paid",
                    CreatedAt = b.CreatedAt
                })
                .OrderByDescending(b => b.CreatedAt)
                .ToList();
        }

        public async Task<IEnumerable<RevenueReportDto>> GetRevenueReportAsync(
            DateTime fromDate, DateTime toDate)
        {
            if (toDate < fromDate)
                throw new ArgumentException(
                    "To date must be after from date.");

            var payments = await _unitOfWork.Payments.GetAllPaymentsAsync();

            var filtered = payments
                .Where(p => p.Status == PaymentStatus.Success &&
                            p.PaidAt.HasValue &&
                            p.PaidAt.Value.Date >= fromDate.Date &&
                            p.PaidAt.Value.Date <= toDate.Date)
                .ToList();

            // Group by month
            var grouped = filtered
                .GroupBy(p => new
                {
                    Year = p.PaidAt.Value.Year,
                    Month = p.PaidAt.Value.Month
                })
                .OrderBy(g => g.Key.Year)
                .ThenBy(g => g.Key.Month)
                .Select(g => new RevenueReportDto
                {
                    Period = $"{new DateTime(g.Key.Year, g.Key.Month, 1):MMMM yyyy}",
                    TotalBookings = g.Count(),
                    TotalRevenue = g.Sum(p => p.Amount),
                    AverageBookingValue = Math.Round(g.Average(p => p.Amount), 2)
                })
                .ToList();

            return grouped;
        }
    }
}