
namespace HotelBooking.Application.DTOs.Promotion
{
    public class PromotionResponseDto
    {
        public int Id { get; set; }
        public int? HotelId { get; set; }
        public string HotelName { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public decimal DiscountPercent { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public int? MaxUsageCount { get; set; }
        public int UsedCount { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
