using AutoMapper;
using HotelBooking.Application.DTOs.Common;
using HotelBooking.Application.DTOs.Hotel;
using HotelBooking.Application.DTOs.Review;
using HotelBooking.Application.DTOs.Room;
using HotelBooking.Application.DTOs.RoomType;
using HotelBooking.Application.Interfaces;
using HotelBooking.Domain.Common;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Interfaces;
// Alias to disambiguate — Application.DTOs.Hotel.HotelSearchDto used in service signature
using AppHotelSearchDto = HotelBooking.Application.DTOs.Hotel.HotelSearchDto;
using DomainHotelSearchDto = HotelBooking.Domain.Common.HotelSearchDto;

namespace HotelBooking.Application.Services
{
    public class HotelService : IHotelService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public HotelService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResponse<HotelResponseDto>> SearchHotelsAsync(AppHotelSearchDto searchDto)
        {
            // Map Application search DTO → Domain search DTO for the repository boundary
            var domainSearch = new DomainHotelSearchDto
            {
                City = searchDto.City,
                Search = searchDto.Search,
                StarRating = searchDto.StarRating,
                MinPrice = searchDto.MinPrice,
                MaxPrice = searchDto.MaxPrice,
                MaxOccupancy = searchDto.MaxOccupancy,
                CheckInDate = searchDto.CheckInDate,
                CheckOutDate = searchDto.CheckOutDate,
                PageNumber = searchDto.PageNumber,
                PageSize = searchDto.PageSize,
                SortBy = searchDto.SortBy,
                SortOrder = searchDto.SortOrder
            };

            var (hotels, totalCount) = await _unitOfWork.Hotels.SearchHotelsAsync(domainSearch);

            return new PagedResponse<HotelResponseDto>
            {
                Data = _mapper.Map<List<HotelResponseDto>>(hotels),
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

            return _mapper.Map<HotelResponseDto>(hotel);
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

            return _mapper.Map<HotelResponseDto>(hotel);
        }

        public async Task<HotelResponseDto> UpdateAsync(int id, UpdateHotelDto dto)
        {
            var hotel = await _unitOfWork.Hotels.GetByIdAsync(id);
            if (hotel == null)
                throw new KeyNotFoundException($"Hotel with ID {id} not found.");

            if (!string.IsNullOrWhiteSpace(dto.Name)) hotel.Name = dto.Name;
            if (!string.IsNullOrWhiteSpace(dto.Description)) hotel.Description = dto.Description;
            if (!string.IsNullOrWhiteSpace(dto.Location)) hotel.Location = dto.Location;
            if (!string.IsNullOrWhiteSpace(dto.City)) hotel.City = dto.City;
            if (!string.IsNullOrWhiteSpace(dto.State)) hotel.State = dto.State;
            if (!string.IsNullOrWhiteSpace(dto.Country)) hotel.Country = dto.Country;
            if (dto.StarRating.HasValue) hotel.StarRating = dto.StarRating.Value;
            if (!string.IsNullOrWhiteSpace(dto.PhoneNumber)) hotel.PhoneNumber = dto.PhoneNumber;
            if (!string.IsNullOrWhiteSpace(dto.Email)) hotel.Email = dto.Email;
            if (!string.IsNullOrWhiteSpace(dto.ImageUrl)) hotel.ImageUrl = dto.ImageUrl;
            if (dto.IsActive.HasValue) hotel.IsActive = dto.IsActive.Value;

            await _unitOfWork.Hotels.UpdateAsync(hotel);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<HotelResponseDto>(hotel);
        }

        public async Task DeleteAsync(int id)
        {
            var hotel = await _unitOfWork.Hotels.GetByIdAsync(id);
            if (hotel == null)
                throw new KeyNotFoundException($"Hotel with ID {id} not found.");

            await _unitOfWork.Hotels.DeleteAsync(hotel);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateImageAsync(int id, string imageUrl)
        {
            var hotel = await _unitOfWork.Hotels.GetByIdAsync(id);
            if (hotel == null)
                throw new KeyNotFoundException($"Hotel with ID {id} not found.");

            hotel.ImageUrl = imageUrl;
            await _unitOfWork.Hotels.UpdateAsync(hotel);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<RoomResponseDto>> GetHotelRoomsAsync(int hotelId)
        {
            var hotel = await _unitOfWork.Hotels.GetHotelWithRoomsAsync(hotelId);
            if (hotel == null)
                throw new KeyNotFoundException($"Hotel with ID {hotelId} not found.");

            var activeRooms = hotel.Rooms.Where(r => !r.IsDeleted).ToList();
            return _mapper.Map<List<RoomResponseDto>>(activeRooms);
        }

        public async Task<IEnumerable<ReviewResponseDto>> GetHotelReviewsAsync(int hotelId)
        {
            var hotel = await _unitOfWork.Hotels.GetHotelWithReviewsAsync(hotelId);
            if (hotel == null)
                throw new KeyNotFoundException($"Hotel with ID {hotelId} not found.");

            var activeReviews = hotel.Reviews.Where(r => !r.IsDeleted).ToList();
            return _mapper.Map<List<ReviewResponseDto>>(activeReviews);
        }

        public async Task<IEnumerable<RoomTypeResponseDto>> GetHotelRoomTypesAsync(int hotelId)
        {
            var hotel = await _unitOfWork.Hotels.GetByIdAsync(hotelId);
            if (hotel == null)
                throw new KeyNotFoundException($"Hotel with ID {hotelId} not found.");

            var roomTypes = await _unitOfWork.RoomTypes.GetByHotelAsync(hotelId);
            return _mapper.Map<List<RoomTypeResponseDto>>(roomTypes);
        }
    }
}