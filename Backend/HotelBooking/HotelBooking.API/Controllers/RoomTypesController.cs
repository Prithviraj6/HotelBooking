using HotelBooking.API.Extensions;
using HotelBooking.Application.DTOs.Common;
using HotelBooking.Application.DTOs.RoomType;
using HotelBooking.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        [Authorize(Roles = "SuperAdmin,HotelAdmin")]
        public async Task<IActionResult> Create([FromBody] CreateRoomTypeDto dto)
        {
            if (User.IsHotelAdmin() && dto.HotelId != User.GetManagedHotelId())
                return Forbid("You can only create room types for the hotel you manage.");

            var result = await _roomTypeService.CreateAsync(dto);
            return StatusCode(201, ApiResponse<RoomTypeResponseDto>.Created(result));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "SuperAdmin,HotelAdmin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateRoomTypeDto dto)
        {
            var roomType = await _roomTypeService.GetByIdAsync(id);
            if (User.IsHotelAdmin() && roomType.HotelId != User.GetManagedHotelId())
                return Forbid("You can only update room types for the hotel you manage.");

            var result = await _roomTypeService.UpdateAsync(id, dto);
            return Ok(ApiResponse<RoomTypeResponseDto>.Ok(result,
                "Room type updated successfully"));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "SuperAdmin,HotelAdmin")]
        public async Task<IActionResult> Delete(int id)
        {
            var roomType = await _roomTypeService.GetByIdAsync(id);
            if (User.IsHotelAdmin() && roomType.HotelId != User.GetManagedHotelId())
                return Forbid("You can only delete room types for the hotel you manage.");

            await _roomTypeService.DeleteAsync(id);
            return Ok(ApiResponse<object>.Ok(null,
                "Room type deleted successfully"));
        }

        [HttpPost("{id}/image")]
        [Authorize(Roles = "SuperAdmin,HotelAdmin")]
        public async Task<IActionResult> UploadImage(int id, IFormFile file, [FromServices] IFileService fileService)
        {
            var roomType = await _roomTypeService.GetByIdAsync(id);
            if (User.IsHotelAdmin() && roomType.HotelId != User.GetManagedHotelId())
                return Forbid("You can only upload images for room types in the hotel you manage.");

            // Upload image
            var relativeUrl = await fileService.UploadImageAsync(file, "roomtypes");
            
            // Update room type record
            await _roomTypeService.UpdateImageAsync(id, relativeUrl);

            return Ok(ApiResponse<string>.Ok(relativeUrl, "Room type image uploaded successfully."));
        }
    }
}