using HotelBooking.Domain.Common;

namespace HotelBooking.Domain.Entities
{
    public class Promotion : BaseEntity
    {
        public int? HotelId { get; set; }   // null = global promo
        public string Code { get; set; }
        public string Description { get; set; }
        public decimal DiscountPercent { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public int? MaxUsageCount { get; set; }
        public int UsedCount { get; set; } = 0;
        public bool IsActive { get; set; } = true;

        // Navigation
        public Hotel Hotel { get; set; }
    }
}
