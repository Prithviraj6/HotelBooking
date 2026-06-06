using HotelBooking.Application.DTOs.Common;
using HotelBooking.Application.DTOs.Review;
using HotelBooking.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HotelBooking.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Create([FromBody] CreateReviewDto dto)
        {
            var userId = GetUserId();
            var result = await _reviewService.CreateAsync(userId, dto);
            return StatusCode(201, ApiResponse<ReviewResponseDto>.Created(result, "Review posted successfully"));
        }

        [HttpGet("hotel/{hotelId}")]
        public async Task<IActionResult> GetByHotel(int hotelId)
        {
            var result = await _reviewService.GetByHotelAsync(hotelId);
            return Ok(ApiResponse<object>.Ok(result, "Reviews retrieved successfully"));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateReviewDto dto)
        {
            var userId = GetUserId();
            var result = await _reviewService.UpdateAsync(id, userId, dto);
            return Ok(ApiResponse<ReviewResponseDto>.Ok(result, "Review updated successfully"));
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();
            var role = GetUserRole();
            await _reviewService.DeleteAsync(id, userId, role);
            return Ok(ApiResponse<object>.Ok(null, "Review deleted successfully"));
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