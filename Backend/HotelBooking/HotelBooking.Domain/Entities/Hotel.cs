using HotelBooking.Domain.Common;

namespace HotelBooking.Domain.Entities
{
    public class Hotel : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public int StarRating { get; set; }
        public string PhoneNumber { get; set; } = "1234567890";
        public string Email { get; set; } = "info@hotel.com";
        public string ImageUrl { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation
        public ICollection<Room> Rooms { get; set; }
        public ICollection<RoomType> RoomTypes { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<Promotion> Promotions { get; set; }
        public ICollection<User> Admins { get; set; }
    }
}
