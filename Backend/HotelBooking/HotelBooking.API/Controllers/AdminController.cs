using HotelBooking.Application.DTOs.Admin;
using HotelBooking.Application.DTOs.Common;
using HotelBooking.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "SuperAdmin,HotelAdmin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            var result = await _adminService.GetDashboardAsync();
            return Ok(ApiResponse<DashboardDto>.Ok(result,
                "Dashboard data retrieved successfully"));
        }

        [HttpGet("reports/bookings")]
        public async Task<IActionResult> GetBookingReport(
            [FromQuery] DateTime fromDate,
            [FromQuery] DateTime toDate)
        {
            if (fromDate == default || toDate == default)
                return BadRequest(ApiResponse<object>.Fail(
                    "From date and to date are required."));

            var result = await _adminService
                .GetBookingReportAsync(fromDate, toDate);

            return Ok(ApiResponse<object>.Ok(result,
                $"Booking report from {fromDate:dd MMM yyyy} " +
                $"to {toDate:dd MMM yyyy}"));
        }

        [HttpGet("reports/revenue")]
        public async Task<IActionResult> GetRevenueReport(
            [FromQuery] DateTime fromDate,
            [FromQuery] DateTime toDate)
        {
            if (fromDate == default || toDate == default)
                return BadRequest(ApiResponse<object>.Fail(
                    "From date and to date are required."));

            var result = await _adminService
                .GetRevenueReportAsync(fromDate, toDate);

            return Ok(ApiResponse<object>.Ok(result,
                $"Revenue report from {fromDate:dd MMM yyyy} " +
                $"to {toDate:dd MMM yyyy}"));
        }

        [HttpPost("hotel-admins")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> CreateHotelAdmin([FromBody] CreateHotelAdminDto dto)
        {
            var result = await _adminService.RegisterHotelAdminAsync(dto);
            return Ok(ApiResponse<HotelAdminResponseDto>.Ok(result, "Hotel Admin successfully registered"));
        }

        [HttpGet("hotel-admins")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> GetHotelAdmins()
        {
            var result = await _adminService.GetHotelAdminsAsync();
            return Ok(ApiResponse<IEnumerable<HotelAdminResponseDto>>.Ok(result, "Hotel Admins retrieved successfully"));
        }

        [HttpDelete("hotel-admins/{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> RevokeHotelAdmin(int id)
        {
            await _adminService.RevokeHotelAdminAsync(id);
            return Ok(ApiResponse<object>.Ok(null, "Hotel Admin access successfully revoked"));
        }
    }
}