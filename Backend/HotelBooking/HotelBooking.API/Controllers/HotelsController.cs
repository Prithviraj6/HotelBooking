using HotelBooking.Application.DTOs.Common;
using HotelBooking.Application.DTOs.Hotel;
using HotelBooking.Application.Interfaces;
using HotelBooking.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HotelsController : ControllerBase
    {
        private readonly IHotelService _hotelService;

        public HotelsController(IHotelService hotelService)
        {
            _hotelService = hotelService;
        }

        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] HotelSearchDto searchDto)
        {
            var result = await _hotelService.SearchHotelsAsync(searchDto);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _hotelService.GetByIdAsync(id);
            return Ok(ApiResponse<HotelResponseDto>.Ok(result));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateHotelDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<object>.Fail(
                    "Validation failed", 400,
                    ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()));

            var result = await _hotelService.CreateAsync(dto);
            return StatusCode(201, ApiResponse<HotelResponseDto>.Created(result));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateHotelDto dto)
        {
            var result = await _hotelService.UpdateAsync(id, dto);
            return Ok(ApiResponse<HotelResponseDto>.Ok(result,
                "Hotel updated successfully"));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _hotelService.DeleteAsync(id);
            return Ok(ApiResponse<object>.Ok(null, "Hotel deleted successfully"));
        }

        [HttpGet("{id}/rooms")]
        public async Task<IActionResult> GetRooms(int id)
        {
            var result = await _hotelService.GetHotelRoomsAsync(id);
            return Ok(ApiResponse<object>.Ok(result, "Rooms retrieved successfully"));
        }

        [HttpGet("{id}/reviews")]
        public async Task<IActionResult> GetReviews(int id)
        {
            var result = await _hotelService.GetHotelReviewsAsync(id);
            return Ok(ApiResponse<object>.Ok(result, "Reviews retrieved successfully"));
        }

        [HttpGet("{id}/room-types")]
        public async Task<IActionResult> GetRoomTypes(int id)
        {
            var result = await _hotelService.GetHotelRoomTypesAsync(id);
            return Ok(ApiResponse<object>.Ok(result,
                "Room types retrieved successfully"));
        }
    }
}