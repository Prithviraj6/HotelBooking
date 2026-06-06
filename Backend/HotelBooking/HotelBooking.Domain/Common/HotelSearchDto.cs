
namespace HotelBooking.Domain.Common
{
    public class HotelSearchDto
    {
        public string? City { get; set; }
        public string? Search { get; set; }
        public int? StarRating { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? MaxOccupancy { get; set; }
        public DateTime? CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; }
        public string? SortOrder { get; set; } = "asc";
    }
}

