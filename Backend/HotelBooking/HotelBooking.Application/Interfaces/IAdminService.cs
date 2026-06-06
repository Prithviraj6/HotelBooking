using HotelBooking.Application.DTOs.Admin;
using HotelBooking.Application.DTOs.Common;

namespace HotelBooking.Application.Interfaces
{
    public interface IAdminService
    {
        Task<DashboardDto> GetDashboardAsync();
        Task<IEnumerable<BookingReportDto>> GetBookingReportAsync(
            DateTime fromDate, DateTime toDate);
        Task<IEnumerable<RevenueReportDto>> GetRevenueReportAsync(
            DateTime fromDate, DateTime toDate);
            
        Task<HotelAdminResponseDto> RegisterHotelAdminAsync(CreateHotelAdminDto dto);
        Task<IEnumerable<HotelAdminResponseDto>> GetHotelAdminsAsync();
        Task RevokeHotelAdminAsync(int adminId);
    }
}