
namespace HotelBooking.Application.DTOs.Common
{
    public class PagedResponse<T>
    {
        public bool Success { get; set; } = true;
        public string Message { get; set; } = "Success";
        public IEnumerable<T> Data { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);
        public bool HasNextPage => PageNumber < TotalPages;
        public bool HasPreviousPage => PageNumber > 1;
    }
}
