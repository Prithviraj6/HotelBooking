using FluentValidation;
using HotelBooking.Application.DTOs.Promotion;

namespace HotelBooking.Application.Validators.Promotion
{
    public class CreatePromotionValidator : AbstractValidator<CreatePromotionDto>
    {
        public CreatePromotionValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Promo code is required.")
                .MaximumLength(50).WithMessage("Promo code cannot exceed 50 characters.")
                .Matches(@"^[A-Za-z0-9_\-]+$")
                    .WithMessage("Promo code can only contain letters, digits, hyphens, and underscores.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.");

            RuleFor(x => x.DiscountPercent)
                .InclusiveBetween(1, 100)
                    .WithMessage("Discount percent must be between 1 and 100.");

            RuleFor(x => x.ValidFrom)
                .NotEmpty().WithMessage("Valid From date is required.");

            RuleFor(x => x.ValidTo)
                .NotEmpty().WithMessage("Valid To date is required.")
                .GreaterThan(x => x.ValidFrom)
                    .WithMessage("Valid To must be after Valid From.");

            RuleFor(x => x.MaxUsageCount)
                .GreaterThan(0).WithMessage("Max usage count must be greater than 0.")
                .When(x => x.MaxUsageCount.HasValue);

            RuleFor(x => x.HotelId)
                .GreaterThan(0).WithMessage("Hotel ID must be greater than 0.")
                .When(x => x.HotelId.HasValue);
        }
    }
}
