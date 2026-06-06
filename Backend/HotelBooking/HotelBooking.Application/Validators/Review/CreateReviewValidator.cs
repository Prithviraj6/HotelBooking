using FluentValidation;
using HotelBooking.Application.DTOs.Review;

namespace HotelBooking.Application.Validators.Review
{
    public class CreateReviewValidator : AbstractValidator<CreateReviewDto>
    {
        public CreateReviewValidator()
        {
            RuleFor(x => x.HotelId)
                .GreaterThan(0).WithMessage("A valid Hotel ID is required.");

            RuleFor(x => x.BookingId)
                .GreaterThan(0).WithMessage("A valid Booking ID is required.");

            RuleFor(x => x.Rating)
                .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5.");

            RuleFor(x => x.Comment)
                .NotEmpty().WithMessage("Comment is required.")
                .MaximumLength(1000).WithMessage("Comment cannot exceed 1000 characters.");
        }
    }
}
