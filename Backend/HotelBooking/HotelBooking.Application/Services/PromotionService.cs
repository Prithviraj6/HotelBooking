using AutoMapper;
using HotelBooking.Application.DTOs.Promotion;
using HotelBooking.Application.Interfaces;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Interfaces;

namespace HotelBooking.Application.Services
{
    public class PromotionService : IPromotionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PromotionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PromotionResponseDto> ValidateAsync(string code, int? hotelId)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Promo code cannot be empty.");

            var promotion = await _unitOfWork.Promotions.GetByCodeAsync(code.ToUpper().Trim());
            if (promotion == null)
                throw new KeyNotFoundException("Promo code not found or inactive.");

            if (promotion.ValidFrom > DateTime.UtcNow)
                throw new InvalidOperationException(
                    $"Promo code is not valid yet. Valid from {promotion.ValidFrom:dd MMM yyyy}.");

            if (promotion.ValidTo < DateTime.UtcNow)
                throw new InvalidOperationException("Promo code has expired.");

            if (promotion.MaxUsageCount.HasValue &&
                promotion.UsedCount >= promotion.MaxUsageCount.Value)
                throw new InvalidOperationException("Promo code has reached its maximum usage limit.");

            if (promotion.HotelId.HasValue)
            {
                if (!hotelId.HasValue)
                    throw new ArgumentException(
                        "This promo code is for a specific hotel. Please provide hotel ID.");

                if (promotion.HotelId != hotelId)
                    throw new InvalidOperationException("Promo code is not valid for this hotel.");
            }

            // Ensure hotel nav prop is resolved for AutoMapper
            if (promotion.HotelId.HasValue && promotion.Hotel == null)
                promotion.Hotel = await _unitOfWork.Hotels.GetByIdAsync(promotion.HotelId.Value);

            return _mapper.Map<PromotionResponseDto>(promotion);
        }

        public async Task<PromotionResponseDto> CreateAsync(CreatePromotionDto dto)
        {
            if (dto.ValidTo <= dto.ValidFrom)
                throw new ArgumentException("Valid To date must be after Valid From date.");

            if (dto.DiscountPercent <= 0 || dto.DiscountPercent > 100)
                throw new ArgumentException("Discount percent must be between 1 and 100.");

            var existing = await _unitOfWork.Promotions.GetByCodeAsync(dto.Code.ToUpper().Trim());
            if (existing != null)
                throw new ArgumentException($"Promo code '{dto.Code}' already exists.");

            Hotel hotelEntity = null;
            if (dto.HotelId.HasValue)
            {
                hotelEntity = await _unitOfWork.Hotels.GetByIdAsync(dto.HotelId.Value);
                if (hotelEntity == null)
                    throw new KeyNotFoundException($"Hotel with ID {dto.HotelId} not found.");
            }

            var promotion = new Promotion
            {
                HotelId = dto.HotelId,
                Code = dto.Code.ToUpper().Trim(),
                Description = dto.Description,
                DiscountPercent = dto.DiscountPercent,
                ValidFrom = dto.ValidFrom,
                ValidTo = dto.ValidTo,
                MaxUsageCount = dto.MaxUsageCount,
                UsedCount = 0,
                IsActive = true,
                Hotel = hotelEntity
            };

            await _unitOfWork.Promotions.AddAsync(promotion);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<PromotionResponseDto>(promotion);
        }

        /// <summary>
        /// Uses GetAllWithHotelAsync — single query with JOIN instead of N+1 per hotel.
        /// </summary>
        public async Task<IEnumerable<PromotionResponseDto>> GetAllAsync()
        {
            var promotions = await _unitOfWork.Promotions.GetAllWithHotelAsync();
            return _mapper.Map<List<PromotionResponseDto>>(promotions);
        }

        public async Task<PromotionResponseDto> UpdateAsync(int id, CreatePromotionDto dto)
        {
            var promotion = await _unitOfWork.Promotions.GetByIdAsync(id);
            if (promotion == null)
                throw new KeyNotFoundException($"Promotion with ID {id} not found.");

            if (!string.IsNullOrWhiteSpace(dto.Code) &&
                dto.Code.ToUpper().Trim() != promotion.Code)
            {
                var existing = await _unitOfWork.Promotions.GetByCodeAsync(dto.Code);
                if (existing != null)
                    throw new ArgumentException($"Promo code '{dto.Code}' already exists.");

                promotion.Code = dto.Code.ToUpper().Trim();
            }

            if (dto.ValidTo <= dto.ValidFrom)
                throw new ArgumentException("Valid To date must be after Valid From date.");

            if (dto.HotelId.HasValue)
            {
                var hotel = await _unitOfWork.Hotels.GetByIdAsync(dto.HotelId.Value);
                if (hotel == null)
                    throw new KeyNotFoundException($"Hotel with ID {dto.HotelId} not found.");
                promotion.HotelId = dto.HotelId;
                promotion.Hotel = hotel;
            }

            promotion.Description = dto.Description;
            promotion.DiscountPercent = dto.DiscountPercent;
            promotion.ValidFrom = dto.ValidFrom;
            promotion.ValidTo = dto.ValidTo;
            promotion.MaxUsageCount = dto.MaxUsageCount;

            await _unitOfWork.Promotions.UpdateAsync(promotion);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<PromotionResponseDto>(promotion);
        }

        public async Task DeleteAsync(int id)
        {
            var promotion = await _unitOfWork.Promotions.GetByIdAsync(id);
            if (promotion == null)
                throw new KeyNotFoundException($"Promotion with ID {id} not found.");

            await _unitOfWork.Promotions.DeleteAsync(promotion);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}