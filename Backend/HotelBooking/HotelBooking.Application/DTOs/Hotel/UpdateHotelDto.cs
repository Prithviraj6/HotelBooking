using System.ComponentModel.DataAnnotations;

namespace HotelBooking.Application.DTOs.Hotel
{
    public class UpdateHotelDto
    {
        [MaxLength(200)]
        public string Name { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }

        [Range(1, 5)]
        public int? StarRating { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string ImageUrl { get; set; }
        public bool? IsActive { get; set; }
    }
}
