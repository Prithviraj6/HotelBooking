
namespace HotelBooking.Application.DTOs.Review
{
    public class ReviewResponseDto
    {
        public int Id { get; set; }
        public int HotelId { get; set; }
        public string HotelName { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int BookingId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
