using FluentValidation;
using HotelBooking.Application.DTOs.Room;

namespace HotelBooking.Application.Validators.Room
{
    public class CreateRoomValidator : AbstractValidator<CreateRoomDto>
    {
        public CreateRoomValidator()
        {
            RuleFor(x => x.HotelId)
                .GreaterThan(0).WithMessage("A valid Hotel ID is required.");

            RuleFor(x => x.RoomTypeId)
                .GreaterThan(0).WithMessage("A valid Room Type ID is required.");

            RuleFor(x => x.RoomNumber)
                .NotEmpty().WithMessage("Room number is required.")
                .MaximumLength(10).WithMessage("Room number cannot exceed 10 characters.");

            RuleFor(x => x.FloorNumber)
                .GreaterThanOrEqualTo(0).WithMessage("Floor number cannot be negative.");
        }
    }
}
