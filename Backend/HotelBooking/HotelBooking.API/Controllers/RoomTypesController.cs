using HotelBooking.Application.DTOs.Common;
using HotelBooking.Application.DTOs.RoomType;
using HotelBooking.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomTypesController : ControllerBase
    {
        private readonly IRoomTypeService _roomTypeService;

        public RoomTypesController(IRoomTypeService roomTypeService)
        {
            _roomTypeService = roomTypeService;
        }

        [HttpGet("hotel/{hotelId}")]
        public async Task<IActionResult> GetByHotel(int hotelId)
        {
            var result = await _roomTypeService.GetByHotelAsync(hotelId);
            return Ok(ApiResponse<object>.Ok(result,
                "Room types retrieved successfully"));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _roomTypeService.GetByIdAsync(id);
            return Ok(ApiResponse<RoomTypeResponseDto>.Ok(result));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateRoomTypeDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<object>.Fail(
                    "Validation failed", 400,
                    ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()));

            var result = await _roomTypeService.CreateAsync(dto);
            return StatusCode(201, ApiResponse<RoomTypeResponseDto>.Created(result));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateRoomTypeDto dto)
        {
            var result = await _roomTypeService.UpdateAsync(id, dto);
            return Ok(ApiResponse<RoomTypeResponseDto>.Ok(result,
                "Room type updated successfully"));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _roomTypeService.DeleteAsync(id);
            return Ok(ApiResponse<object>.Ok(null,
                "Room type deleted successfully"));
        }
    }
}