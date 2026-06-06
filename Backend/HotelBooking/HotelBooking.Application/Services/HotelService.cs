using HotelBooking.Application.DTOs.Common;
using HotelBooking.Application.DTOs.Hotel;
using HotelBooking.Application.DTOs.Review;
using HotelBooking.Application.DTOs.Room;
using HotelBooking.Application.DTOs.RoomType;
using HotelBooking.Application.Interfaces;
using HotelBooking.Domain.Common;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Interfaces;

namespace HotelBooking.Application.Services
{
    public class HotelService : IHotelService
    {
        private readonly IUnitOfWork _unitOfWork;

        public HotelService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedResponse<HotelResponseDto>> SearchHotelsAsync(
            HotelSearchDto searchDto)
        {
            var (hotels, totalCount) = await _unitOfWork.Hotels
                .SearchHotelsAsync(searchDto);

            var data = hotels.Select(h => MapToResponseDto(h)).ToList();

            return new PagedResponse<HotelResponseDto>
            {
                Data = data,
                TotalRecords = totalCount,
                PageNumber = searchDto.PageNumber,
                PageSize = searchDto.PageSize,
                Message = "Hotels retrieved successfully"
            };
        }

        public async Task<HotelResponseDto> GetByIdAsync(int id)
        {
            var hotel = await _unitOfWork.Hotels.GetHotelWithRoomsAsync(id);
            if (hotel == null)
                throw new KeyNotFoundException($"Hotel with ID {id} not found.");

            return MapToResponseDto(hotel);
        }

        public async Task<HotelResponseDto> CreateAsync(CreateHotelDto dto)
        {
            var hotel = new Hotel
            {
                Name = dto.Name,
                Description = dto.Description,
                Location = dto.Location,
                City = dto.City,
                State = dto.State,
                Country = dto.Country,
                StarRating = dto.StarRating,
                PhoneNumber = dto.PhoneNumber,
                Email = dto.Email,
                ImageUrl = dto.ImageUrl,
                IsActive = true
            };

            await _unitOfWork.Hotels.AddAsync(hotel);
            await _unitOfWork.SaveChangesAsync();

            return MapToResponseDto(hotel);
        }

        public async Task<HotelResponseDto> UpdateAsync(int id, UpdateHotelDto dto)
        {
            var hotel = await _unitOfWork.Hotels.GetByIdAsync(id);
            if (hotel == null)
                throw new KeyNotFoundException($"Hotel with ID {id} not found.");

            if (!string.IsNullOrWhiteSpace(dto.Name))
                hotel.Name = dto.Name;

            if (!string.IsNullOrWhiteSpace(dto.Description))
                hotel.Description = dto.Description;

            if (!string.IsNullOrWhiteSpace(dto.Location))
                hotel.Location = dto.Location;

            if (!string.IsNullOrWhiteSpace(dto.City))
                hotel.City = dto.City;

            if (!string.IsNullOrWhiteSpace(dto.State))
                hotel.State = dto.State;

            if (!string.IsNullOrWhiteSpace(dto.Country))
                hotel.Country = dto.Country;

            if (dto.StarRating.HasValue)
                hotel.StarRating = dto.StarRating.Value;

            if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
                hotel.PhoneNumber = dto.PhoneNumber;

            if (!string.IsNullOrWhiteSpace(dto.Email))
                hotel.Email = dto.Email;

            if (!string.IsNullOrWhiteSpace(dto.ImageUrl))
                hotel.ImageUrl = dto.ImageUrl;

            if (dto.IsActive.HasValue)
                hotel.IsActive = dto.IsActive.Value;

            await _unitOfWork.Hotels.UpdateAsync(hotel);
            await _unitOfWork.SaveChangesAsync();

            return MapToResponseDto(hotel);
        }

        public async Task DeleteAsync(int id)
        {
            var hotel = await _unitOfWork.Hotels.GetByIdAsync(id);
            if (hotel == null)
                throw new KeyNotFoundException($"Hotel with ID {id} not found.");

            await _unitOfWork.Hotels.DeleteAsync(hotel);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<RoomResponseDto>> GetHotelRoomsAsync(int hotelId)
        {
            var hotel = await _unitOfWork.Hotels.GetHotelWithRoomsAsync(hotelId);
            if (hotel == null)
                throw new KeyNotFoundException($"Hotel with ID {hotelId} not found.");

            return hotel.Rooms
                .Where(r => !r.IsDeleted)
                .Select(r => new RoomResponseDto
                {
                    Id = r.Id,
                    HotelId = r.HotelId,
                    HotelName = hotel.Name,
                    RoomTypeId = r.RoomTypeId,
                    RoomTypeName = r.RoomType?.TypeName,
                    Category = r.RoomType?.Category.ToString(),
                    RoomNumber = r.RoomNumber,
                    FloorNumber = r.FloorNumber,
                    Status = r.Status.ToString(),
                    PricePerNight = r.RoomType?.PricePerNight ?? 0,
                    MaxOccupancy = r.RoomType?.MaxOccupancy ?? 0,
                    Amenities = r.RoomType?.Amenities,
                    CreatedAt = r.CreatedAt
                }).ToList();
        }

        public async Task<IEnumerable<ReviewResponseDto>> GetHotelReviewsAsync(int hotelId)
        {
            var hotel = await _unitOfWork.Hotels.GetHotelWithReviewsAsync(hotelId);
            if (hotel == null)
                throw new KeyNotFoundException($"Hotel with ID {hotelId} not found.");

            return hotel.Reviews
                .Where(r => !r.IsDeleted)
                .Select(r => new ReviewResponseDto
                {
                    Id = r.Id,
                    HotelId = r.HotelId,
                    HotelName = hotel.Name,
                    UserId = r.UserId,
                    UserName = $"{r.User?.FirstName} {r.User?.LastName}",
                    BookingId = r.BookingId,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt
                }).ToList();
        }

        public async Task<IEnumerable<RoomTypeResponseDto>> GetHotelRoomTypesAsync(
            int hotelId)
        {
            var hotel = await _unitOfWork.Hotels.GetByIdAsync(hotelId);
            if (hotel == null)
                throw new KeyNotFoundException($"Hotel with ID {hotelId} not found.");

            var roomTypes = await _unitOfWork.RoomTypes.GetByHotelAsync(hotelId);

            return roomTypes.Select(rt => new RoomTypeResponseDto
            {
                Id = rt.Id,
                HotelId = rt.HotelId,
                HotelName = hotel.Name,
                TypeName = rt.TypeName,
                Category = rt.Category.ToString(),
                Description = rt.Description,
                PricePerNight = rt.PricePerNight,
                MaxOccupancy = rt.MaxOccupancy,
                Amenities = rt.Amenities,
                ImageUrl = rt.ImageUrl,
                CreatedAt = rt.CreatedAt
            }).ToList();
        }

        // ─── Private Mapper ──────────────────────────────────────────────

        private static HotelResponseDto MapToResponseDto(Hotel hotel)
        {
            var reviews = hotel.Reviews?.Where(r => !r.IsDeleted).ToList();
            var avgRating = reviews != null && reviews.Any()
                ? reviews.Average(r => r.Rating)
                : 0.0;

            return new HotelResponseDto
            {
                Id = hotel.Id,
                Name = hotel.Name,
                Description = hotel.Description,
                Location = hotel.Location,
                City = hotel.City,
                State = hotel.State,
                Country = hotel.Country,
                StarRating = hotel.StarRating,
                PhoneNumber = hotel.PhoneNumber,
                Email = hotel.Email,
                ImageUrl = hotel.ImageUrl,
                IsActive = hotel.IsActive,
                AverageRating = Math.Round(avgRating, 1),
                TotalReviews = reviews?.Count ?? 0,
                CreatedAt = hotel.CreatedAt
            };
        }
    }
}