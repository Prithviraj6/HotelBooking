using AutoMapper;
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
        private readonly IMapper _mapper;

        public RoomService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Uses GetAllRoomsWithDetailsAsync — single SQL JOIN query (fixes N+1).
        /// </summary>
        public async Task<IEnumerable<RoomResponseDto>> GetAllAsync()
        {
            var rooms = await _unitOfWork.Rooms.GetAllRoomsWithDetailsAsync();
            return _mapper.Map<List<RoomResponseDto>>(rooms);
        }

        public async Task<RoomResponseDto> GetByIdAsync(int id)
        {
            var room = await _unitOfWork.Rooms.GetByIdAsync(id);
            if (room == null)
                throw new KeyNotFoundException($"Room with ID {id} not found.");

            var hotel = await _unitOfWork.Hotels.GetByIdAsync(room.HotelId);
            var roomType = await _unitOfWork.RoomTypes.GetByIdAsync(room.RoomTypeId);
            room.Hotel = hotel;
            room.RoomType = roomType;

            return _mapper.Map<RoomResponseDto>(room);
        }

        public async Task<IEnumerable<RoomResponseDto>> GetAvailableRoomsAsync(
            int hotelId, DateTime checkIn, DateTime checkOut)
        {
            if (checkIn.Date < DateTime.UtcNow.Date)
                throw new ArgumentException("Check-in date cannot be in the past.");

            if (checkOut.Date <= checkIn.Date)
                throw new ArgumentException("Check-out date must be after check-in date.");

            var hotel = await _unitOfWork.Hotels.GetByIdAsync(hotelId);
            if (hotel == null)
                throw new KeyNotFoundException($"Hotel with ID {hotelId} not found.");

            var rooms = await _unitOfWork.Rooms.GetAvailableRoomsAsync(hotelId, checkIn, checkOut);

            // Attach hotel to each room so AutoMapper can resolve HotelName
            foreach (var r in rooms) r.Hotel = hotel;

            return _mapper.Map<List<RoomResponseDto>>(rooms);
        }

        public async Task<RoomResponseDto> CreateAsync(CreateRoomDto dto)
        {
            var hotel = await _unitOfWork.Hotels.GetByIdAsync(dto.HotelId);
            if (hotel == null)
                throw new KeyNotFoundException($"Hotel with ID {dto.HotelId} not found.");

            var roomType = await _unitOfWork.RoomTypes.GetByIdAsync(dto.RoomTypeId);
            if (roomType == null)
                throw new KeyNotFoundException($"Room type with ID {dto.RoomTypeId} not found.");

            if (roomType.HotelId != dto.HotelId)
                throw new ArgumentException("Room type does not belong to the specified hotel.");

            var existingRooms = await _unitOfWork.Rooms.GetRoomsByHotelAsync(dto.HotelId);
            if (existingRooms.Any(r => r.RoomNumber.ToLower() == dto.RoomNumber.ToLower() && !r.IsDeleted))
                throw new ArgumentException($"Room number {dto.RoomNumber} already exists in this hotel.");

            var room = new Room
            {
                HotelId = dto.HotelId,
                RoomTypeId = dto.RoomTypeId,
                RoomNumber = dto.RoomNumber,
                FloorNumber = dto.FloorNumber,
                Status = RoomStatus.Available,
                Hotel = hotel,
                RoomType = roomType
            };

            await _unitOfWork.Rooms.AddAsync(room);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<RoomResponseDto>(room);
        }

        public async Task<RoomResponseDto> UpdateAsync(int id, UpdateRoomDto dto)
        {
            var room = await _unitOfWork.Rooms.GetByIdAsync(id);
            if (room == null)
                throw new KeyNotFoundException($"Room with ID {id} not found.");

            if (!string.IsNullOrWhiteSpace(dto.RoomNumber) && dto.RoomNumber != room.RoomNumber)
            {
                var existingRooms = await _unitOfWork.Rooms.GetRoomsByHotelAsync(room.HotelId);
                if (existingRooms.Any(r =>
                    r.RoomNumber.ToLower() == dto.RoomNumber.ToLower() &&
                    r.Id != id && !r.IsDeleted))
                    throw new ArgumentException($"Room number {dto.RoomNumber} already exists in this hotel.");

                room.RoomNumber = dto.RoomNumber;
            }

            if (dto.FloorNumber.HasValue) room.FloorNumber = dto.FloorNumber.Value;
            if (dto.Status.HasValue) room.Status = dto.Status.Value;

            await _unitOfWork.Rooms.UpdateAsync(room);
            await _unitOfWork.SaveChangesAsync();

            var hotel = await _unitOfWork.Hotels.GetByIdAsync(room.HotelId);
            var roomType = await _unitOfWork.RoomTypes.GetByIdAsync(room.RoomTypeId);
            room.Hotel = hotel;
            room.RoomType = roomType;

            return _mapper.Map<RoomResponseDto>(room);
        }

        public async Task DeleteAsync(int id)
        {
            var room = await _unitOfWork.Rooms.GetByIdAsync(id);
            if (room == null)
                throw new KeyNotFoundException($"Room with ID {id} not found.");

            var isAvailable = await _unitOfWork.Rooms.IsRoomAvailableAsync(
                id, DateTime.UtcNow, DateTime.UtcNow.AddYears(1));

            if (!isAvailable)
                throw new InvalidOperationException("Cannot delete room with active or upcoming bookings.");

            await _unitOfWork.Rooms.DeleteAsync(room);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateStatusAsync(int id, RoomStatus status)
        {
            var room = await _unitOfWork.Rooms.GetByIdAsync(id);
            if (room == null)
                throw new KeyNotFoundException($"Room with ID {id} not found.");

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
    }
}