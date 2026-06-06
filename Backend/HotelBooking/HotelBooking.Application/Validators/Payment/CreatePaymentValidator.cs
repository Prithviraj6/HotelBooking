using FluentValidation;
using HotelBooking.Application.DTOs.Payment;
using HotelBooking.Domain.Enums;

namespace HotelBooking.Application.Validators.Payment
{
    public class CreatePaymentValidator : AbstractValidator<CreatePaymentDto>
    {
        public CreatePaymentValidator()
        {
            RuleFor(x => x.BookingId)
                .GreaterThan(0).WithMessage("A valid Booking ID is required.");

            RuleFor(x => x.Method)
                .IsInEnum().WithMessage($"Payment method must be one of: {string.Join(", ", Enum.GetNames<PaymentMethod>())}.");

            RuleFor(x => x.TransactionId)
                .NotEmpty().WithMessage("Transaction ID is required.")
                .MaximumLength(100).WithMessage("Transaction ID cannot exceed 100 characters.");
        }
    }
}
