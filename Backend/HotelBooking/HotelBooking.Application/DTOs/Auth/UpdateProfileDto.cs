using System.ComponentModel.DataAnnotations;

namespace HotelBooking.Application.DTOs.Auth
{
    public class UpdateProfileDto
    {
        [MaxLength(100)]
        public string? FirstName { get; set; }

        [MaxLength(100)]
        public string? LastName { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }
    }
}
