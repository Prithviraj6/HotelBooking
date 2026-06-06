using HotelBooking.API.Extensions;
using HotelBooking.Application.DTOs.Common;
using HotelBooking.Application.DTOs.Hotel;
using HotelBooking.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Create([FromBody] CreateHotelDto dto)
        {
            // FluentValidation runs automatically via AddFluentValidationAutoValidation()
            // No manual ModelState check needed
            var result = await _hotelService.CreateAsync(dto);
            return StatusCode(201, ApiResponse<HotelResponseDto>.Created(result));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "SuperAdmin,HotelAdmin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateHotelDto dto)
        {
            if (User.IsHotelAdmin() && User.GetManagedHotelId() != id)
                return Forbid("You can only update the hotel you manage.");

            var result = await _hotelService.UpdateAsync(id, dto);
            return Ok(ApiResponse<HotelResponseDto>.Ok(result, "Hotel updated successfully"));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _hotelService.DeleteAsync(id);
            return Ok(ApiResponse<object>.Ok(null, "Hotel deleted successfully"));
        }

        [HttpPost("{id}/image")]
        [Authorize(Roles = "SuperAdmin,HotelAdmin")]
        public async Task<IActionResult> UploadImage(int id, IFormFile file, [FromServices] IFileService fileService)
        {
            if (User.IsHotelAdmin() && User.GetManagedHotelId() != id)
                return Forbid("You can only upload images for the hotel you manage.");

            // Upload image
            var relativeUrl = await fileService.UploadImageAsync(file, "hotels");
            
            // Update hotel record
            await _hotelService.UpdateImageAsync(id, relativeUrl);

            return Ok(ApiResponse<string>.Ok(relativeUrl, "Hotel image uploaded successfully."));
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
            return Ok(ApiResponse<object>.Ok(result, "Room types retrieved successfully"));
        }
    }
}