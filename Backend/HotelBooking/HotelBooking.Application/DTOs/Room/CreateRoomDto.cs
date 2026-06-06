using System.ComponentModel.DataAnnotations;

namespace HotelBooking.Application.DTOs.Room
{
    public class CreateRoomDto
    {
        [Required]
        public int HotelId { get; set; }

        [Required]
        public int RoomTypeId { get; set; }

        [Required]
        [MaxLength(10)]
        public string RoomNumber { get; set; }

        [Required]
        public int FloorNumber { get; set; }
    }
}
