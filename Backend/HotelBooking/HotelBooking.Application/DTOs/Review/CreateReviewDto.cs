using System.ComponentModel.DataAnnotations;

namespace HotelBooking.Application.DTOs.Review
{
    public class CreateReviewDto
    {
        [Required]
        public int HotelId { get; set; }

        [Required]
        public int BookingId { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Comment { get; set; }
    }
}
