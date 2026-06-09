using FluentValidation;
using HotelBooking.Application.DTOs.Booking;

namespace HotelBooking.Application.Validators.Booking
{
    public class CreateBookingValidator : AbstractValidator<CreateBookingDto>
    {
        public CreateBookingValidator()
        {
            RuleFor(x => x.RoomTypeId)
                .GreaterThan(0).WithMessage("A valid Room Type ID is required.");

            RuleFor(x => x.CheckInDate)
                .NotEmpty().WithMessage("Check-in date is required.")
                .GreaterThanOrEqualTo(DateTime.UtcNow.Date)
                    .WithMessage("Check-in date cannot be in the past.");

            RuleFor(x => x.CheckOutDate)
                .NotEmpty().WithMessage("Check-out date is required.")
                .GreaterThan(x => x.CheckInDate)
                    .WithMessage("Check-out date must be after check-in date.");

            RuleFor(x => x.SpecialRequests)
                .MaximumLength(500).WithMessage("Special requests cannot exceed 500 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.SpecialRequests));

            RuleFor(x => x.PromoCode)
                .MaximumLength(20).WithMessage("Promo code cannot exceed 20 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.PromoCode));
        }
    }
}
