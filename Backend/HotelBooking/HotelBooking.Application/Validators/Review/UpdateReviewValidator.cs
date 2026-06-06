using FluentValidation;
using HotelBooking.Application.DTOs.Review;

namespace HotelBooking.Application.Validators.Review
{
    public class UpdateReviewValidator : AbstractValidator<UpdateReviewDto>
    {
        public UpdateReviewValidator()
        {
            RuleFor(x => x.Rating)
                .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5.")
                .When(x => x.Rating.HasValue);

            RuleFor(x => x.Comment)
                .MaximumLength(1000).WithMessage("Comment cannot exceed 1000 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Comment));
        }
    }
}
