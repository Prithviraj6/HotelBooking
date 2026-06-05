using HotelBooking.Domain.Common;
using HotelBooking.Domain.Enums;

namespace HotelBooking.Domain.Entities
{
    public class Payment : BaseEntity
    {
        public int BookingId { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethod Method { get; set; }
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public string TransactionId { get; set; }
        public DateTime? PaidAt { get; set; }

        // Navigation
        public Booking Booking { get; set; }
    }
}
