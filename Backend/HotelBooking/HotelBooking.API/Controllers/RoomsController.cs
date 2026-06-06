using HotelBooking.Application.DTOs.Common;
using HotelBooking.Application.DTOs.Room;
using HotelBooking.Application.Interfaces;
using HotelBooking.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomService _roomService;

        public RoomsController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _roomService.GetAllAsync();
            return Ok(ApiResponse<object>.Ok(result,
                "Rooms retrieved successfully"));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _roomService.GetByIdAsync(id);
            return Ok(ApiResponse<RoomResponseDto>.Ok(result));
        }

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailable(
            [FromQuery] int hotelId,
            [FromQuery] DateTime checkIn,
            [FromQuery] DateTime checkOut)
        {
            if (hotelId <= 0)
                return BadRequest(ApiResponse<object>.Fail(
                    "Valid hotel ID is required."));

            if (checkIn == default || checkOut == default)
                return BadRequest(ApiResponse<object>.Fail(
                    "Check-in and check-out dates are required."));

            var result = await _roomService.GetAvailableRoomsAsync(
                hotelId, checkIn, checkOut);

            return Ok(ApiResponse<object>.Ok(result,
                "Available rooms retrieved successfully"));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateRoomDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<object>.Fail(
                    "Validation failed", 400,
                    ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()));

            var result = await _roomService.CreateAsync(dto);
            return StatusCode(201, ApiResponse<RoomResponseDto>.Created(result));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateRoomDto dto)
        {
            var result = await _roomService.UpdateAsync(id, dto);
            return Ok(ApiResponse<RoomResponseDto>.Ok(result,
                "Room updated successfully"));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _roomService.DeleteAsync(id);
            return Ok(ApiResponse<object>.Ok(null, "Room deleted successfully"));
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(
            int id, [FromBody] RoomStatus status)
        {
            await _roomService.UpdateStatusAsync(id, status);
            return Ok(ApiResponse<object>.Ok(null,
                "Room status updated successfully"));
        }
    }
}