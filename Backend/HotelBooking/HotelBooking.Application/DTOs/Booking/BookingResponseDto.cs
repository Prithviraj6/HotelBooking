
namespace HotelBooking.Application.DTOs.Booking
{
    public class BookingResponseDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public int RoomId { get; set; }
        public string RoomNumber { get; set; }
        public string RoomType { get; set; }
        public string HotelName { get; set; }
        public string HotelCity { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int TotalNights { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal DiscountAmount { get; set; }
        public string PromoCode { get; set; }
        public string Status { get; set; }
        public string SpecialRequests { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
