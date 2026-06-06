using FluentValidation;
using HotelBooking.Application.DTOs.Hotel;

namespace HotelBooking.Application.Validators.Hotel
{
    public class UpdateHotelValidator : AbstractValidator<UpdateHotelDto>
    {
        public UpdateHotelValidator()
        {
            RuleFor(x => x.Name)
                .MaximumLength(200).WithMessage("Hotel name cannot exceed 200 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Name));

            RuleFor(x => x.City)
                .MaximumLength(100).WithMessage("City cannot exceed 100 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.City));

            RuleFor(x => x.Country)
                .MaximumLength(100).WithMessage("Country cannot exceed 100 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Country));

            RuleFor(x => x.StarRating)
                .InclusiveBetween(1, 5).WithMessage("Star rating must be between 1 and 5.")
                .When(x => x.StarRating.HasValue);

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("A valid email address is required.")
                .MaximumLength(200).WithMessage("Email cannot exceed 200 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Email));

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^\+?[0-9]{7,15}$").WithMessage("Invalid phone number format.")
                .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));
        }
    }
}
