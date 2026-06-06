using FluentValidation;
using HotelBooking.Application.DTOs.RoomType;

namespace HotelBooking.Application.Validators.RoomType
{
    public class UpdateRoomTypeValidator : AbstractValidator<UpdateRoomTypeDto>
    {
        public UpdateRoomTypeValidator()
        {
            RuleFor(x => x.TypeName)
                .MaximumLength(100).WithMessage("Room type name cannot exceed 100 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.TypeName));

            RuleFor(x => x.Category)
                .IsInEnum().WithMessage("Invalid room category value.")
                .When(x => x.Category.HasValue);

            RuleFor(x => x.PricePerNight)
                .GreaterThan(0).WithMessage("Price per night must be greater than 0.")
                .LessThanOrEqualTo(100000).WithMessage("Price per night cannot exceed 100,000.")
                .When(x => x.PricePerNight.HasValue);

            RuleFor(x => x.MaxOccupancy)
                .InclusiveBetween(1, 20).WithMessage("Max occupancy must be between 1 and 20.")
                .When(x => x.MaxOccupancy.HasValue);
        }
    }
}
