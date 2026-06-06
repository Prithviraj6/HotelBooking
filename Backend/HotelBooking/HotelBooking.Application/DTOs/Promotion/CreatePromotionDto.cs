using System.ComponentModel.DataAnnotations;

namespace HotelBooking.Application.DTOs.Promotion
{
    public class CreatePromotionDto
    {
        public int? HotelId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Code { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [Range(1, 100)]
        public decimal DiscountPercent { get; set; }

        [Required]
        public DateTime ValidFrom { get; set; }

        [Required]
        public DateTime ValidTo { get; set; }

        public int? MaxUsageCount { get; set; }
    }
}
