using HotelBooking.Domain.Enums;

namespace HotelBooking.Application.DTOs.Room
{
    public class UpdateRoomDto
    {
        public string RoomNumber { get; set; }
        public int? FloorNumber { get; set; }
        public RoomStatus? Status { get; set; }
    }
}
