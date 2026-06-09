using System;

namespace HotelBooking.Application.DTOs.Admin
{
    public class UserDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public int? HotelId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
