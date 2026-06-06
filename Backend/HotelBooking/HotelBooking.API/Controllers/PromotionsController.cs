using HotelBooking.Application.DTOs.Common;
using HotelBooking.Application.DTOs.Promotion;
using HotelBooking.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PromotionsController : ControllerBase
    {
        private readonly IPromotionService _promotionService;

        public PromotionsController(IPromotionService promotionService)
        {
            _promotionService = promotionService;
        }

        [HttpGet("validate")]
        [Authorize]
        public async Task<IActionResult> Validate(
            [FromQuery] string code,
            [FromQuery] int? hotelId)
        {
            if (string.IsNullOrWhiteSpace(code))
                return BadRequest(ApiResponse<object>.Fail(
                    "Promo code is required."));

            var result = await _promotionService.ValidateAsync(code, hotelId);
            return Ok(ApiResponse<PromotionResponseDto>.Ok(result,
                $"Promo code is valid. You get {result.DiscountPercent}% discount!"));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _promotionService.GetAllAsync();
            return Ok(ApiResponse<object>.Ok(result,
                "Promotions retrieved successfully"));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreatePromotionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<object>.Fail(
                    "Validation failed", 400,
                    ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()));

            var result = await _promotionService.CreateAsync(dto);
            return StatusCode(201, ApiResponse<PromotionResponseDto>.Created(result,
                "Promotion created successfully"));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(
            int id, [FromBody] CreatePromotionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<object>.Fail(
                    "Validation failed", 400,
                    ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()));

            var result = await _promotionService.UpdateAsync(id, dto);
            return Ok(ApiResponse<PromotionResponseDto>.Ok(result,
                "Promotion updated successfully"));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _promotionService.DeleteAsync(id);
            return Ok(ApiResponse<object>.Ok(null,
                "Promotion deleted successfully"));
        }
    }
}