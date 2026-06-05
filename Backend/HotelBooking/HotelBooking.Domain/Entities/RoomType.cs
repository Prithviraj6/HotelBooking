using HotelBooking.Domain.Common;
using HotelBooking.Domain.Enums;

namespace HotelBooking.Domain.Entities
{
    public class RoomType : BaseEntity
    {
        public int HotelId { get; set; }
        public string TypeName { get; set; }
        public RoomCategory Category { get; set; }
        public string Description { get; set; }
        public decimal PricePerNight { get; set; }
        public int MaxOccupancy { get; set; }
        public string Amenities { get; set; }
        public string ImageUrl { get; set; }

        // Navigation
        public Hotel Hotel { get; set; }
        public ICollection<Room> Rooms { get; set; }
    }
}
