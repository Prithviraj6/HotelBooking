
namespace HotelBooking.Application.DTOs.Room
{
    public class RoomResponseDto
    {
        public int Id { get; set; }
        public int HotelId { get; set; }
        public string HotelName { get; set; }
        public int RoomTypeId { get; set; }
        public string RoomTypeName { get; set; }
        public string Category { get; set; }
        public string RoomNumber { get; set; }
        public int FloorNumber { get; set; }
        public string Status { get; set; }
        public decimal PricePerNight { get; set; }
        public int MaxOccupancy { get; set; }
        public string Amenities { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
