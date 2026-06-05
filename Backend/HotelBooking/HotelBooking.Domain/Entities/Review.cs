using HotelBooking.Domain.Common;

namespace HotelBooking.Domain.Entities
{
    public class Review : BaseEntity
    {
        public int HotelId { get; set; }
        public int UserId { get; set; }
        public int BookingId { get; set; }
        public int Rating { get; set; }   // 1-5
        public string Comment { get; set; }

        // Navigation
        public Hotel Hotel { get; set; }
        public User User { get; set; }
        public Booking Booking { get; set; }
    }
}
