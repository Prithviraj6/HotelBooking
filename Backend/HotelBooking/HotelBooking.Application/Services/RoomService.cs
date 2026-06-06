using HotelBooking.Application.DTOs.Room;
using HotelBooking.Application.Interfaces;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Enums;
using HotelBooking.Domain.Interfaces;

namespace HotelBooking.Application.Services
{
    public class RoomService : IRoomService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RoomService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<RoomResponseDto>> GetAllAsync()
        {
            var rooms = await _unitOfWork.Rooms.GetAllAsync();
            var result = new List<RoomResponseDto>();

            foreach (var room in rooms)
            {
                var hotel = await _unitOfWork.Hotels.GetByIdAsync(room.HotelId);
                var roomType = await _unitOfWork.RoomTypes.GetByIdAsync(room.RoomTypeId);
                result.Add(MapToResponseDto(room, hotel?.Name, roomType));
            }

            return result;
        }

        public async Task<RoomResponseDto> GetByIdAsync(int id)
        {
            var room = await _unitOfWork.Rooms.GetByIdAsync(id);
            if (room == null)
                throw new KeyNotFoundException($"Room with ID {id} not found.");

            var hotel = await _unitOfWork.Hotels.GetByIdAsync(room.HotelId);
            var roomType = await _unitOfWork.RoomTypes.GetByIdAsync(room.RoomTypeId);

            return MapToResponseDto(room, hotel?.Name, roomType);
        }

        public async Task<IEnumerable<RoomResponseDto>> GetAvailableRoomsAsync(
            int hotelId, DateTime checkIn, DateTime checkOut)
        {
            // Validate dates
            if (checkIn.Date < DateTime.UtcNow.Date)
                throw new ArgumentException("Check-in date cannot be in the past.");

            if (checkOut.Date <= checkIn.Date)
                throw new ArgumentException(
                    "Check-out date must be after check-in date.");

            // Check hotel exists
            var hotel = await _unitOfWork.Hotels.GetByIdAsync(hotelId);
            if (hotel == null)
                throw new KeyNotFoundException($"Hotel with ID {hotelId} not found.");

            var rooms = await _unitOfWork.Rooms
                .GetAvailableRoomsAsync(hotelId, checkIn, checkOut);

            return rooms.Select(r =>
                MapToResponseDto(r, hotel.Name, r.RoomType)).ToList();
        }

        public async Task<RoomResponseDto> CreateAsync(CreateRoomDto dto)
        {
            // Check hotel exists
            var hotel = await _unitOfWork.Hotels.GetByIdAsync(dto.HotelId);
            if (hotel == null)
                throw new KeyNotFoundException($"Hotel with ID {dto.HotelId} not found.");

            // Check room type exists and belongs to same hotel
            var roomType = await _unitOfWork.RoomTypes.GetByIdAsync(dto.RoomTypeId);
            if (roomType == null)
                throw new KeyNotFoundException(
                    $"Room type with ID {dto.RoomTypeId} not found.");

            if (roomType.HotelId != dto.HotelId)
                throw new ArgumentException(
                    "Room type does not belong to the specified hotel.");

            // Check room number is unique within hotel
            var existingRooms = await _unitOfWork.Rooms.GetRoomsByHotelAsync(dto.HotelId);
            var roomNumberExists = existingRooms.Any(r =>
                r.RoomNumber.ToLower() == dto.RoomNumber.ToLower() && !r.IsDeleted);

            if (roomNumberExists)
                throw new ArgumentException(
                    $"Room number {dto.RoomNumber} already exists in this hotel.");

            var room = new Room
            {
                HotelId = dto.HotelId,
                RoomTypeId = dto.RoomTypeId,
                RoomNumber = dto.RoomNumber,
                FloorNumber = dto.FloorNumber,
                Status = RoomStatus.Available
            };

            await _unitOfWork.Rooms.AddAsync(room);
            await _unitOfWork.SaveChangesAsync();

            return MapToResponseDto(room, hotel.Name, roomType);
        }

        public async Task<RoomResponseDto> UpdateAsync(int id, UpdateRoomDto dto)
        {
            var room = await _unitOfWork.Rooms.GetByIdAsync(id);
            if (room == null)
                throw new KeyNotFoundException($"Room with ID {id} not found.");

            // If changing room number check uniqueness
            if (!string.IsNullOrWhiteSpace(dto.RoomNumber) &&
                dto.RoomNumber != room.RoomNumber)
            {
                var existingRooms = await _unitOfWork.Rooms
                    .GetRoomsByHotelAsync(room.HotelId);
                var roomNumberExists = existingRooms.Any(r =>
                    r.RoomNumber.ToLower() == dto.RoomNumber.ToLower() &&
                    r.Id != id &&
                    !r.IsDeleted);

                if (roomNumberExists)
                    throw new ArgumentException(
                        $"Room number {dto.RoomNumber} already exists in this hotel.");

                room.RoomNumber = dto.RoomNumber;
            }

            if (dto.FloorNumber.HasValue)
                room.FloorNumber = dto.FloorNumber.Value;

            if (dto.Status.HasValue)
                room.Status = dto.Status.Value;

            await _unitOfWork.Rooms.UpdateAsync(room);
            await _unitOfWork.SaveChangesAsync();

            var hotel = await _unitOfWork.Hotels.GetByIdAsync(room.HotelId);
            var roomType = await _unitOfWork.RoomTypes.GetByIdAsync(room.RoomTypeId);

            return MapToResponseDto(room, hotel?.Name, roomType);
        }

        public async Task DeleteAsync(int id)
        {
            var room = await _unitOfWork.Rooms.GetByIdAsync(id);
            if (room == null)
                throw new KeyNotFoundException($"Room with ID {id} not found.");

            // Cannot delete room with active bookings
            var isAvailable = await _unitOfWork.Rooms.IsRoomAvailableAsync(
                id, DateTime.UtcNow, DateTime.UtcNow.AddYears(1));

            if (!isAvailable)
                throw new InvalidOperationException(
                    "Cannot delete room with active or upcoming bookings.");

            await _unitOfWork.Rooms.DeleteAsync(room);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateStatusAsync(int id, RoomStatus status)
        {
            var room = await _unitOfWork.Rooms.GetByIdAsync(id);
            if (room == null)
                throw new KeyNotFoundException($"Room with ID {id} not found.");

            // Cannot mark as available if it has active bookings
            if (status == RoomStatus.Available)
            {
                var hasActiveBookings = !await _unitOfWork.Rooms.IsRoomAvailableAsync(
                    id, DateTime.UtcNow, DateTime.UtcNow.AddDays(1));

                if (hasActiveBookings)
                    throw new InvalidOperationException(
                        "Room has active bookings and cannot be marked as available.");
            }

            room.Status = status;
            await _unitOfWork.Rooms.UpdateAsync(room);
            await _unitOfWork.SaveChangesAsync();
        }

        // ─── Private Mapper ──────────────────────────────────────────────

        private static RoomResponseDto MapToResponseDto(
            Room room, string hotelName, RoomType roomType)
        {
            return new RoomResponseDto
            {
                Id = room.Id,
                HotelId = room.HotelId,
                HotelName = hotelName,
                RoomTypeId = room.RoomTypeId,
                RoomTypeName = roomType?.TypeName,
                Category = roomType?.Category.ToString(),
                RoomNumber = room.RoomNumber,
                FloorNumber = room.FloorNumber,
                Status = room.Status.ToString(),
                PricePerNight = roomType?.PricePerNight ?? 0,
                MaxOccupancy = roomType?.MaxOccupancy ?? 0,
                Amenities = roomType?.Amenities,
                CreatedAt = room.CreatedAt
            };
        }
    }
}