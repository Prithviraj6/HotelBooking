
namespace HotelBooking.Application.DTOs.RoomType
{
    public class RoomTypeResponseDto
    {
        public int Id { get; set; }
        public int HotelId { get; set; }
        public string HotelName { get; set; }
        public string TypeName { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public decimal PricePerNight { get; set; }
        public int MaxOccupancy { get; set; }
        public string Amenities { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
