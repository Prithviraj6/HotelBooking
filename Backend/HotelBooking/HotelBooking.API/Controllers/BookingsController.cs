using HotelBooking.Application.DTOs.Booking;
using HotelBooking.Application.DTOs.Common;
using HotelBooking.Application.Interfaces;
using HotelBooking.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HotelBooking.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Create([FromBody] CreateBookingDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<object>.Fail(
                    "Validation failed", 400,
                    ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()));

            var userId = GetUserId();
            var result = await _bookingService.CreateAsync(userId, dto);
            return StatusCode(201, ApiResponse<BookingResponseDto>.Created(result,
                "Booking created successfully"));
        }

        [HttpGet("my")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetMyBookings()
        {
            var userId = GetUserId();
            var result = await _bookingService.GetMyBookingsAsync(userId);
            return Ok(ApiResponse<object>.Ok(result,
                "Bookings retrieved successfully"));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = GetUserId();
            var role = GetUserRole();
            var result = await _bookingService.GetByIdAsync(id, userId, role);
            return Ok(ApiResponse<BookingResponseDto>.Ok(result));
        }

        [HttpPut("{id}/cancel")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = GetUserId();
            var result = await _bookingService.CancelAsync(id, userId);
            return Ok(ApiResponse<BookingResponseDto>.Ok(result,
                "Booking cancelled successfully"));
        }

        [HttpGet("history")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetHistory()
        {
            var userId = GetUserId();
            var result = await _bookingService.GetMyBookingsAsync(userId);
            return Ok(ApiResponse<object>.Ok(result,
                "Booking history retrieved successfully"));
        }

        [HttpPost("{id}/rebook")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Rebook(int id)
        {
            var userId = GetUserId();
            var result = await _bookingService.RebookAsync(id, userId);
            return StatusCode(201, ApiResponse<BookingResponseDto>.Created(result,
                "Rebooking successful"));
        }

        // ─── Admin Endpoints ─────────────────────────────────────────────

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _bookingService.GetAllAsync();
            return Ok(ApiResponse<object>.Ok(result,
                "All bookings retrieved successfully"));
        }

        [HttpPut("{id}/confirm")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Confirm(int id)
        {
            var result = await _bookingService
                .UpdateStatusAsync(id, BookingStatus.Confirmed);
            return Ok(ApiResponse<BookingResponseDto>.Ok(result,
                "Booking confirmed successfully"));
        }

        [HttpPut("{id}/complete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Complete(int id)
        {
            var result = await _bookingService
                .UpdateStatusAsync(id, BookingStatus.Completed);
            return Ok(ApiResponse<BookingResponseDto>.Ok(result,
                "Booking marked as completed"));
        }

        [HttpPut("{id}/no-show")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> NoShow(int id)
        {
            var result = await _bookingService
                .UpdateStatusAsync(id, BookingStatus.NoShow);
            return Ok(ApiResponse<BookingResponseDto>.Ok(result,
                "Booking marked as no-show"));
        }

        // ─── Helpers ─────────────────────────────────────────────────────

        private int GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(claim))
                throw new UnauthorizedAccessException("User not authenticated.");
            return int.Parse(claim);
        }

        private string GetUserRole()
        {
            return User.FindFirst(ClaimTypes.Role)?.Value ?? "Customer";
        }
    }
}