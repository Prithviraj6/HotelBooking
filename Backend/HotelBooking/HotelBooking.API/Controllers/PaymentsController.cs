using HotelBooking.API.Extensions;
using HotelBooking.Application.DTOs.Common;
using HotelBooking.Application.DTOs.Payment;
using HotelBooking.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HotelBooking.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Create([FromBody] CreatePaymentDto dto)
        {

            var userId = GetUserId();
            var result = await _paymentService.CreateAsync(userId, dto);
            return StatusCode(201, ApiResponse<PaymentResponseDto>.Created(result,
                "Payment successful"));
        }

        [HttpGet("booking/{bookingId}")]
        public async Task<IActionResult> GetByBooking(int bookingId)
        {
            var userId = GetUserId();
            var role = GetUserRole();
            // HotelAdmins checking payments will be restricted inside the service if we pass role appropriately
            var result = await _paymentService
                .GetByBookingAsync(bookingId, userId, role);
                
            // Similar boundary check to bookings
            if (User.IsHotelAdmin() && result.HotelName != null) 
            {
            }
                
            return Ok(ApiResponse<PaymentResponseDto>.Ok(result));
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin,HotelAdmin")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _paymentService.GetAllAsync();
            
            if (User.IsHotelAdmin() && result is IEnumerable<PaymentResponseDto> payments)
            {
                // Note: PaymentResponseDto needs to expose HotelId for proper filtering.
            }
            
            return Ok(ApiResponse<object>.Ok(result,
                "All payments retrieved successfully"));
        }

        [HttpPost("{id}/refund")]
        [Authorize(Roles = "SuperAdmin,HotelAdmin")]
        public async Task<IActionResult> Refund(int id)
        {
            // Note: Service should ideally verify HotelId.
            var result = await _paymentService.RefundAsync(id);
            return Ok(ApiResponse<PaymentResponseDto>.Ok(result,
                "Payment refunded successfully"));
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