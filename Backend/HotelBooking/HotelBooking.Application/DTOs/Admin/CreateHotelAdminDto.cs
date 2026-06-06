using System.ComponentModel.DataAnnotations;

namespace HotelBooking.Application.DTOs.Admin
{
    public class CreateHotelAdminDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public int HotelId { get; set; }
    }
}
