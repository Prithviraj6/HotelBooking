using HotelBooking.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace HotelBooking.Application.DTOs.RoomType
{
    public class UpdateRoomTypeDto
    {
        public string TypeName { get; set; }
        public RoomCategory? Category { get; set; }
        public string Description { get; set; }

        [Range(1, 100000)]
        public decimal? PricePerNight { get; set; }

        [Range(1, 10)]
        public int? MaxOccupancy { get; set; }
        public string Amenities { get; set; }
        public string ImageUrl { get; set; }
    }
}
