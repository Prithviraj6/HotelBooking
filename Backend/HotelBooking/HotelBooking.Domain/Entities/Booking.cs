using HotelBooking.Domain.Common;
using HotelBooking.Domain.Enums;

namespace HotelBooking.Domain.Entities
{
    public class Booking : BaseEntity
    {
        public int UserId { get; set; }
        public int RoomId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int TotalNights { get; set; }
        public decimal TotalPrice { get; set; }
        public BookingStatus Status { get; set; } = BookingStatus.Pending;
        public string SpecialRequests { get; set; }
        public string? PromoCode { get; set; }
        public decimal DiscountAmount { get; set; } = 0;

        // Navigation
        public User User { get; set; }
        public Room Room { get; set; }
        public Payment Payment { get; set; }
        public Review Review { get; set; }
    }
}
