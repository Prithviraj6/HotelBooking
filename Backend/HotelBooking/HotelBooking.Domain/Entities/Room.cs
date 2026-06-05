using HotelBooking.Domain.Common;
using HotelBooking.Domain.Enums;

namespace HotelBooking.Domain.Entities
{
    public class Room : BaseEntity
    {
        public int HotelId { get; set; }
        public int RoomTypeId { get; set; }
        public string RoomNumber { get; set; }
        public int FloorNumber { get; set; }
        public RoomStatus Status { get; set; } = RoomStatus.Available;

        // Navigation
        public Hotel Hotel { get; set; }
        public RoomType RoomType { get; set; }
        public ICollection<Booking> Bookings { get; set; }
    }
}
