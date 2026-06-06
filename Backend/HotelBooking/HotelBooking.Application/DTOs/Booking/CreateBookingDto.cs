using System.ComponentModel.DataAnnotations;

namespace HotelBooking.Application.DTOs.Booking
{
    public class CreateBookingDto
    {
        [Required]
        public int RoomId { get; set; }

        [Required]
        public DateTime CheckInDate { get; set; }

        [Required]
        public DateTime CheckOutDate { get; set; }

        public string SpecialRequests { get; set; }
        public string PromoCode { get; set; }
    }
}
