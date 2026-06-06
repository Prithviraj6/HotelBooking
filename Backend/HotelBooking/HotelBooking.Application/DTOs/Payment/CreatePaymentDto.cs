using HotelBooking.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace HotelBooking.Application.DTOs.Payment
{
    public class CreatePaymentDto
    {
        [Required]
        public int BookingId { get; set; }

        [Required]
        public PaymentMethod Method { get; set; }

        [Required]
        public string TransactionId { get; set; }
    }
}
