using FluentValidation;
using HotelBooking.Application.DTOs.Room;

namespace HotelBooking.Application.Validators.Room
{
    public class UpdateRoomValidator : AbstractValidator<UpdateRoomDto>
    {
        public UpdateRoomValidator()
        {
            RuleFor(x => x.RoomNumber)
                .MaximumLength(10).WithMessage("Room number cannot exceed 10 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.RoomNumber));

            RuleFor(x => x.FloorNumber)
                .GreaterThanOrEqualTo(0).WithMessage("Floor number cannot be negative.")
                .When(x => x.FloorNumber.HasValue);
        }
    }
}
