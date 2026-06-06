using AutoMapper;
using HotelBooking.Application.DTOs.RoomType;
using HotelBooking.Application.Interfaces;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Interfaces;

namespace HotelBooking.Application.Services
{
    public class RoomTypeService : IRoomTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RoomTypeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RoomTypeResponseDto>> GetByHotelAsync(int hotelId)
        {
            var hotel = await _unitOfWork.Hotels.GetByIdAsync(hotelId);
            if (hotel == null)
                throw new KeyNotFoundException($"Hotel with ID {hotelId} not found.");

            var roomTypes = await _unitOfWork.RoomTypes.GetByHotelAsync(hotelId);

            // Attach hotel nav prop so AutoMapper can resolve HotelName
            foreach (var rt in roomTypes) rt.Hotel = hotel;

            return _mapper.Map<List<RoomTypeResponseDto>>(roomTypes);
        }

        public async Task<RoomTypeResponseDto> GetByIdAsync(int id)
        {
            var roomType = await _unitOfWork.RoomTypes.GetByIdAsync(id);
            if (roomType == null)
                throw new KeyNotFoundException($"Room type with ID {id} not found.");

            var hotel = await _unitOfWork.Hotels.GetByIdAsync(roomType.HotelId);
            roomType.Hotel = hotel;

            return _mapper.Map<RoomTypeResponseDto>(roomType);
        }

        public async Task<RoomTypeResponseDto> CreateAsync(CreateRoomTypeDto dto)
        {
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
                ImageUrl = dto.ImageUrl,
                Hotel = hotel
            };

            await _unitOfWork.RoomTypes.AddAsync(roomType);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<RoomTypeResponseDto>(roomType);
        }

        public async Task<RoomTypeResponseDto> UpdateAsync(int id, UpdateRoomTypeDto dto)
        {
            var roomType = await _unitOfWork.RoomTypes.GetByIdAsync(id);
            if (roomType == null)
                throw new KeyNotFoundException($"Room type with ID {id} not found.");

            if (!string.IsNullOrWhiteSpace(dto.TypeName)) roomType.TypeName = dto.TypeName;
            if (dto.Category.HasValue) roomType.Category = dto.Category.Value;
            if (!string.IsNullOrWhiteSpace(dto.Description)) roomType.Description = dto.Description;
            if (dto.PricePerNight.HasValue) roomType.PricePerNight = dto.PricePerNight.Value;
            if (dto.MaxOccupancy.HasValue) roomType.MaxOccupancy = dto.MaxOccupancy.Value;
            if (!string.IsNullOrWhiteSpace(dto.Amenities)) roomType.Amenities = dto.Amenities;
            if (!string.IsNullOrWhiteSpace(dto.ImageUrl)) roomType.ImageUrl = dto.ImageUrl;

            await _unitOfWork.RoomTypes.UpdateAsync(roomType);
            await _unitOfWork.SaveChangesAsync();

            var hotel = await _unitOfWork.Hotels.GetByIdAsync(roomType.HotelId);
            roomType.Hotel = hotel;

            return _mapper.Map<RoomTypeResponseDto>(roomType);
        }

        public async Task UpdateImageAsync(int id, string imageUrl)
        {
            var roomType = await _unitOfWork.RoomTypes.GetByIdAsync(id);
            if (roomType == null)
                throw new KeyNotFoundException($"Room type with ID {id} not found.");

            roomType.ImageUrl = imageUrl;
            await _unitOfWork.RoomTypes.UpdateAsync(roomType);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var roomType = await _unitOfWork.RoomTypes.GetByIdAsync(id);
            if (roomType == null)
                throw new KeyNotFoundException($"Room type with ID {id} not found.");

            var rooms = await _unitOfWork.Rooms.GetRoomsByHotelAsync(roomType.HotelId);
            if (rooms.Any(r => r.RoomTypeId == id && !r.IsDeleted))
                throw new InvalidOperationException(
                    "Cannot delete room type that has active rooms assigned to it.");

            await _unitOfWork.RoomTypes.DeleteAsync(roomType);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}