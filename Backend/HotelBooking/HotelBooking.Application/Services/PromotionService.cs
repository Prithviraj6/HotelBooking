using HotelBooking.Application.DTOs.Promotion;
using HotelBooking.Application.Interfaces;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Interfaces;

namespace HotelBooking.Application.Services
{
    public class PromotionService : IPromotionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PromotionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PromotionResponseDto> ValidateAsync(string code, int? hotelId)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Promo code cannot be empty.");

            var promotion = await _unitOfWork.Promotions.GetByCodeAsync(code);

            if (promotion == null)
                throw new KeyNotFoundException("Promo code not found or inactive.");

            // Check validity dates
            if (promotion.ValidFrom > DateTime.UtcNow)
                throw new InvalidOperationException(
                    $"Promo code is not valid yet. Valid from {promotion.ValidFrom:dd MMM yyyy}.");

            if (promotion.ValidTo < DateTime.UtcNow)
                throw new InvalidOperationException("Promo code has expired.");

            // Check usage limit
            if (promotion.MaxUsageCount.HasValue &&
                promotion.UsedCount >= promotion.MaxUsageCount.Value)
                throw new InvalidOperationException(
                    "Promo code has reached its maximum usage limit.");

            // Check hotel specific promo
            if (promotion.HotelId.HasValue)
            {
                if (!hotelId.HasValue)
                    throw new ArgumentException(
                        "This promo code is for a specific hotel. Please provide hotel ID.");

                if (promotion.HotelId != hotelId)
                    throw new InvalidOperationException(
                        "Promo code is not valid for this hotel.");
            }

            var hotelName = promotion.Hotel?.Name;
            if (promotion.HotelId.HasValue && hotelName == null)
            {
                var hotel = await _unitOfWork.Hotels
                    .GetByIdAsync(promotion.HotelId.Value);
                hotelName = hotel?.Name;
            }

            return MapToResponseDto(promotion, hotelName);
        }

        public async Task<PromotionResponseDto> CreateAsync(CreatePromotionDto dto)
        {
            // Validate dates
            if (dto.ValidTo <= dto.ValidFrom)
                throw new ArgumentException(
                    "Valid To date must be after Valid From date.");

            if (dto.DiscountPercent <= 0 || dto.DiscountPercent > 100)
                throw new ArgumentException(
                    "Discount percent must be between 1 and 100.");

            // Check promo code is unique
            var existing = await _unitOfWork.Promotions.GetByCodeAsync(dto.Code);
            if (existing != null)
                throw new ArgumentException(
                    $"Promo code '{dto.Code}' already exists.");

            // If hotel specific check hotel exists
            string hotelName = null;
            if (dto.HotelId.HasValue)
            {
                var hotel = await _unitOfWork.Hotels.GetByIdAsync(dto.HotelId.Value);
                if (hotel == null)
                    throw new KeyNotFoundException(
                        $"Hotel with ID {dto.HotelId} not found.");
                hotelName = hotel.Name;
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
                IsActive = true
            };

            await _unitOfWork.Promotions.AddAsync(promotion);
            await _unitOfWork.SaveChangesAsync();

            return MapToResponseDto(promotion, hotelName);
        }

        public async Task<IEnumerable<PromotionResponseDto>> GetAllAsync()
        {
            var promotions = await _unitOfWork.Promotions.GetAllAsync();
            var result = new List<PromotionResponseDto>();

            foreach (var promotion in promotions)
            {
                string hotelName = null;
                if (promotion.HotelId.HasValue)
                {
                    var hotel = await _unitOfWork.Hotels
                        .GetByIdAsync(promotion.HotelId.Value);
                    hotelName = hotel?.Name;
                }
                result.Add(MapToResponseDto(promotion, hotelName));
            }

            return result;
        }

        public async Task<PromotionResponseDto> UpdateAsync(int id, CreatePromotionDto dto)
        {
            var promotion = await _unitOfWork.Promotions.GetByIdAsync(id);
            if (promotion == null)
                throw new KeyNotFoundException(
                    $"Promotion with ID {id} not found.");

            // If code is changing check uniqueness
            if (!string.IsNullOrWhiteSpace(dto.Code) &&
                dto.Code.ToUpper().Trim() != promotion.Code)
            {
                var existing = await _unitOfWork.Promotions
                    .GetByCodeAsync(dto.Code);
                if (existing != null)
                    throw new ArgumentException(
                        $"Promo code '{dto.Code}' already exists.");

                promotion.Code = dto.Code.ToUpper().Trim();
            }

            // Validate dates
            if (dto.ValidTo <= dto.ValidFrom)
                throw new ArgumentException(
                    "Valid To date must be after Valid From date.");

            // If hotel specific check hotel exists
            string hotelName = null;
            if (dto.HotelId.HasValue)
            {
                var hotel = await _unitOfWork.Hotels.GetByIdAsync(dto.HotelId.Value);
                if (hotel == null)
                    throw new KeyNotFoundException(
                        $"Hotel with ID {dto.HotelId} not found.");
                hotelName = hotel.Name;
                promotion.HotelId = dto.HotelId;
            }

            promotion.Description = dto.Description;
            promotion.DiscountPercent = dto.DiscountPercent;
            promotion.ValidFrom = dto.ValidFrom;
            promotion.ValidTo = dto.ValidTo;
            promotion.MaxUsageCount = dto.MaxUsageCount;

            await _unitOfWork.Promotions.UpdateAsync(promotion);
            await _unitOfWork.SaveChangesAsync();

            return MapToResponseDto(promotion, hotelName);
        }

        public async Task DeleteAsync(int id)
        {
            var promotion = await _unitOfWork.Promotions.GetByIdAsync(id);
            if (promotion == null)
                throw new KeyNotFoundException(
                    $"Promotion with ID {id} not found.");

            await _unitOfWork.Promotions.DeleteAsync(promotion);
            await _unitOfWork.SaveChangesAsync();
        }

        // ─── Private Mapper ──────────────────────────────────────────────

        private static PromotionResponseDto MapToResponseDto(
            Promotion promotion, string hotelName)
        {
            return new PromotionResponseDto
            {
                Id = promotion.Id,
                HotelId = promotion.HotelId,
                HotelName = hotelName,
                Code = promotion.Code,
                Description = promotion.Description,
                DiscountPercent = promotion.DiscountPercent,
                ValidFrom = promotion.ValidFrom,
                ValidTo = promotion.ValidTo,
                MaxUsageCount = promotion.MaxUsageCount,
                UsedCount = promotion.UsedCount,
                IsActive = promotion.IsActive,
                CreatedAt = promotion.CreatedAt
            };
        }
    }
}