using AutoMapper;
using HotelBooking.Application.DTOs.Admin;
using HotelBooking.Application.DTOs.Common;
using HotelBooking.Application.Interfaces;
using HotelBooking.Domain.Enums;
using HotelBooking.Domain.Interfaces;

namespace HotelBooking.Application.Services
{
    public class AdminService : IAdminService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        public AdminService(IUnitOfWork unitOfWork, IMapper mapper, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _emailService = emailService;
        }

        public async Task<HotelAdminResponseDto> RegisterHotelAdminAsync(HotelBooking.Application.DTOs.Admin.CreateHotelAdminDto dto)
        {
            var emailExists = await _unitOfWork.Users.EmailExistsAsync(dto.Email);
            if (emailExists)
                throw new ArgumentException("Email is already registered.");

            var hotel = await _unitOfWork.Hotels.GetByIdAsync(dto.HotelId);
            if (hotel == null)
                throw new KeyNotFoundException("Hotel not found.");

            var user = new HotelBooking.Domain.Entities.User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email.ToLower().Trim(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                PhoneNumber = dto.PhoneNumber,
                Role = HotelBooking.Domain.Enums.UserRole.HotelAdmin,
                ManagedHotelId = dto.HotelId
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            // Optional: send registration confirmation
            await _emailService.SendRegistrationConfirmationAsync(user.Email, user.FirstName);

            return new HotelBooking.Application.DTOs.Admin.HotelAdminResponseDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                ManagedHotelId = hotel.Id,
                ManagedHotelName = hotel.Name
            };
        }

        public async Task<IEnumerable<HotelBooking.Application.DTOs.Admin.HotelAdminResponseDto>> GetHotelAdminsAsync()
        {
            var users = await _unitOfWork.Users.GetAllAsync();
            var hotelAdmins = users.Where(u => u.Role == HotelBooking.Domain.Enums.UserRole.HotelAdmin && u.ManagedHotelId.HasValue).ToList();
            
            var response = new List<HotelBooking.Application.DTOs.Admin.HotelAdminResponseDto>();
            foreach (var admin in hotelAdmins)
            {
                var hotel = await _unitOfWork.Hotels.GetByIdAsync(admin.ManagedHotelId!.Value);
                response.Add(new HotelBooking.Application.DTOs.Admin.HotelAdminResponseDto
                {
                    Id = admin.Id,
                    FirstName = admin.FirstName,
                    LastName = admin.LastName,
                    Email = admin.Email,
                    PhoneNumber = admin.PhoneNumber,
                    ManagedHotelId = hotel?.Id ?? 0,
                    ManagedHotelName = hotel?.Name ?? "Unknown"
                });
            }
            return response;
        }

        public async Task RevokeHotelAdminAsync(int adminId)
        {
            var admin = await _unitOfWork.Users.GetByIdAsync(adminId);
            if (admin == null || admin.Role != HotelBooking.Domain.Enums.UserRole.HotelAdmin)
                throw new KeyNotFoundException("Hotel admin not found.");

            await _unitOfWork.Users.DeleteAsync(admin);
            await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// All counts/sums are computed at the database level via IDashboardRepository.
        /// Queries run concurrently — no full table scans in memory.
        /// </summary>
        public async Task<DashboardDto> GetDashboardAsync()
        {
            var db = _unitOfWork.Dashboard;

            // DbContext is not thread-safe, so we must await these sequentially.
            // Since these are DB-level scalar queries, performance is still excellent.
            var totalHotels = await db.GetTotalHotelsAsync();
            var totalRooms = await db.GetTotalRoomsAsync();
            var totalUsers = await db.GetTotalUsersAsync();
            var totalBookings = await db.GetTotalBookingsAsync();
            var pending = await db.GetBookingsByStatusAsync(BookingStatus.Pending);
            var confirmed = await db.GetBookingsByStatusAsync(BookingStatus.Confirmed);
            var completed = await db.GetBookingsByStatusAsync(BookingStatus.Completed);
            var cancelled = await db.GetBookingsByStatusAsync(BookingStatus.Cancelled);
            var bookingsThisMonth = await db.GetBookingsThisMonthAsync();
            var totalRevenue = await db.GetTotalRevenueAsync();
            var revenueThisMonth = await db.GetRevenueThisMonthAsync();
            var occupancy = await db.GetOccupancyRateAsync();

            return new DashboardDto
            {
                TotalHotels = totalHotels,
                TotalRooms = totalRooms,
                TotalUsers = totalUsers,
                TotalBookings = totalBookings,
                PendingBookings = pending,
                ConfirmedBookings = confirmed,
                CompletedBookings = completed,
                CancelledBookings = cancelled,
                BookingsThisMonth = bookingsThisMonth,
                TotalRevenue = totalRevenue,
                RevenueThisMonth = revenueThisMonth,
                AverageOccupancyRate = occupancy
            };
        }

        public async Task<IEnumerable<BookingReportDto>> GetBookingReportAsync(
            DateTime fromDate, DateTime toDate)
        {
            if (toDate < fromDate)
                throw new ArgumentException("To date must be after from date.");

            var bookings = await _unitOfWork.Bookings.GetAllBookingsAsync();

            return bookings
                .Where(b => b.CreatedAt.Date >= fromDate.Date && b.CreatedAt.Date <= toDate.Date)
                .Select(b => new BookingReportDto
                {
                    BookingId = b.Id,
                    UserName = b.User != null ? $"{b.User.FirstName} {b.User.LastName}" : "—",
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
                throw new ArgumentException("To date must be after from date.");

            var payments = await _unitOfWork.Payments.GetAllPaymentsAsync();

            return payments
                .Where(p => p.Status == PaymentStatus.Success &&
                            p.PaidAt.HasValue &&
                            p.PaidAt.Value.Date >= fromDate.Date &&
                            p.PaidAt.Value.Date <= toDate.Date)
                .GroupBy(p => new { p.PaidAt!.Value.Year, p.PaidAt.Value.Month })
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
        }
    }
}