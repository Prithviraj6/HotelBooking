
namespace HotelBooking.Application.DTOs.Auth
{
    public class AuthResponseDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }
        public DateTime TokenExpiry { get; set; }
    }
}
