using FluentValidation;
using HotelBooking.Application.DTOs.RoomType;
using HotelBooking.Domain.Enums;

namespace HotelBooking.Application.Validators.RoomType
{
    public class CreateRoomTypeValidator : AbstractValidator<CreateRoomTypeDto>
    {
        public CreateRoomTypeValidator()
        {
            RuleFor(x => x.HotelId)
                .GreaterThan(0).WithMessage("A valid Hotel ID is required.");

            RuleFor(x => x.TypeName)
                .NotEmpty().WithMessage("Room type name is required.")
                .MaximumLength(100).WithMessage("Room type name cannot exceed 100 characters.");

            RuleFor(x => x.Category)
                .IsInEnum().WithMessage($"Category must be one of: {string.Join(", ", Enum.GetNames<RoomCategory>())}.");

            RuleFor(x => x.PricePerNight)
                .GreaterThan(0).WithMessage("Price per night must be greater than 0.")
                .LessThanOrEqualTo(100000).WithMessage("Price per night cannot exceed 100,000.");

            RuleFor(x => x.MaxOccupancy)
                .InclusiveBetween(1, 20).WithMessage("Max occupancy must be between 1 and 20.");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Description));
        }
    }
}
