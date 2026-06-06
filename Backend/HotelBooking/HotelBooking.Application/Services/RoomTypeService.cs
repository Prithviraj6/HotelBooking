using HotelBooking.Application.DTOs.RoomType;
using HotelBooking.Application.Interfaces;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Interfaces;

namespace HotelBooking.Application.Services
{
    public class RoomTypeService : IRoomTypeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RoomTypeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<RoomTypeResponseDto>> GetByHotelAsync(int hotelId)
        {
            // Check hotel exists
            var hotel = await _unitOfWork.Hotels.GetByIdAsync(hotelId);
            if (hotel == null)
                throw new KeyNotFoundException($"Hotel with ID {hotelId} not found.");

            var roomTypes = await _unitOfWork.RoomTypes.GetByHotelAsync(hotelId);

            return roomTypes.Select(rt => MapToResponseDto(rt, hotel.Name)).ToList();
        }

        public async Task<RoomTypeResponseDto> GetByIdAsync(int id)
        {
            var roomType = await _unitOfWork.RoomTypes.GetByIdAsync(id);
            if (roomType == null)
                throw new KeyNotFoundException($"Room type with ID {id} not found.");

            var hotel = await _unitOfWork.Hotels.GetByIdAsync(roomType.HotelId);

            return MapToResponseDto(roomType, hotel?.Name);
        }

        public async Task<RoomTypeResponseDto> CreateAsync(CreateRoomTypeDto dto)
        {
            // Check hotel exists
            var hotel = await _unitOfWork.Hotels.GetByIdAsync(dto.HotelId);
            if (hotel == null)
                throw new KeyNotFoundException($"Hotel with ID {dto.HotelId} not found.");

            var roomType = new RoomType
            {
                HotelId = dto.HotelId,
                TypeName = dto.TypeName,
                Category = dto.Category,
                Description = dto.Description,
                PricePerNight = dto.PricePerNight,
                MaxOccupancy = dto.MaxOccupancy,
                Amenities = dto.Amenities,
                ImageUrl = dto.ImageUrl
            };

            await _unitOfWork.RoomTypes.AddAsync(roomType);
            await _unitOfWork.SaveChangesAsync();

            return MapToResponseDto(roomType, hotel.Name);
        }

        public async Task<RoomTypeResponseDto> UpdateAsync(int id, UpdateRoomTypeDto dto)
        {
            var roomType = await _unitOfWork.RoomTypes.GetByIdAsync(id);
            if (roomType == null)
                throw new KeyNotFoundException($"Room type with ID {id} not found.");

            if (!string.IsNullOrWhiteSpace(dto.TypeName))
                roomType.TypeName = dto.TypeName;

            if (dto.Category.HasValue)
                roomType.Category = dto.Category.Value;

            if (!string.IsNullOrWhiteSpace(dto.Description))
                roomType.Description = dto.Description;

            if (dto.PricePerNight.HasValue)
                roomType.PricePerNight = dto.PricePerNight.Value;

            if (dto.MaxOccupancy.HasValue)
                roomType.MaxOccupancy = dto.MaxOccupancy.Value;

            if (!string.IsNullOrWhiteSpace(dto.Amenities))
                roomType.Amenities = dto.Amenities;

            if (!string.IsNullOrWhiteSpace(dto.ImageUrl))
                roomType.ImageUrl = dto.ImageUrl;

            await _unitOfWork.RoomTypes.UpdateAsync(roomType);
            await _unitOfWork.SaveChangesAsync();

            var hotel = await _unitOfWork.Hotels.GetByIdAsync(roomType.HotelId);
            return MapToResponseDto(roomType, hotel?.Name);
        }

        public async Task DeleteAsync(int id)
        {
            var roomType = await _unitOfWork.RoomTypes.GetByIdAsync(id);
            if (roomType == null)
                throw new KeyNotFoundException($"Room type with ID {id} not found.");

            // Check if any active rooms are using this room type
            var rooms = await _unitOfWork.Rooms.GetRoomsByHotelAsync(roomType.HotelId);
            var hasActiveRooms = rooms.Any(r => r.RoomTypeId == id && !r.IsDeleted);
            if (hasActiveRooms)
                throw new InvalidOperationException(
                    "Cannot delete room type that has active rooms assigned to it.");

            await _unitOfWork.RoomTypes.DeleteAsync(roomType);
            await _unitOfWork.SaveChangesAsync();
        }

        // ─── Private Mapper ──────────────────────────────────────────────

        private static RoomTypeResponseDto MapToResponseDto(
            RoomType roomType, string hotelName)
        {
            return new RoomTypeResponseDto
            {
                Id = roomType.Id,
                HotelId = roomType.HotelId,
                HotelName = hotelName,
                TypeName = roomType.TypeName,
                Category = roomType.Category.ToString(),
                Description = roomType.Description,
                PricePerNight = roomType.PricePerNight,
                MaxOccupancy = roomType.MaxOccupancy,
                Amenities = roomType.Amenities,
                ImageUrl = roomType.ImageUrl,
                CreatedAt = roomType.CreatedAt
            };
        }
    }
}