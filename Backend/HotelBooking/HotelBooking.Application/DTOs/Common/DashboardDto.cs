namespace HotelBooking.Application.DTOs.Common
{
    public class DashboardDto
    {
        public int TotalHotels { get; set; }
        public int TotalRooms { get; set; }
        public int TotalUsers { get; set; }
        public int TotalBookings { get; set; }
        public int PendingBookings { get; set; }
        public int ConfirmedBookings { get; set; }
        public int CompletedBookings { get; set; }
        public int CancelledBookings { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal RevenueThisMonth { get; set; }
        public int BookingsThisMonth { get; set; }
        public double AverageOccupancyRate { get; set; }
    }

    public class RevenueReportDto
    {
        public string Period { get; set; }
        public int TotalBookings { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageBookingValue { get; set; }
    }

    public class BookingReportDto
    {
        public int BookingId { get; set; }
        public string UserName { get; set; }
        public string HotelName { get; set; }
        public string RoomNumber { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int TotalNights { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}