namespace HotelBooking.Application.DTOs.Admin
{
    public class HotelAdminResponseDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int ManagedHotelId { get; set; }
        public string ManagedHotelName { get; set; }
    }
}
