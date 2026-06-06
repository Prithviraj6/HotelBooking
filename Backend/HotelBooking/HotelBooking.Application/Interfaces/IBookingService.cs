using HotelBooking.Application.DTOs.Booking;
using HotelBooking.Domain.Enums;

namespace HotelBooking.Application.Interfaces
{
    public interface IBookingService
    {
        Task<BookingResponseDto> CreateAsync(int userId, CreateBookingDto dto);
        Task<IEnumerable<BookingResponseDto>> GetMyBookingsAsync(int userId);
        Task<BookingResponseDto> GetByIdAsync(int bookingId, int userId, string role);
        Task<BookingResponseDto> CancelAsync(int bookingId, int userId);
        Task<IEnumerable<BookingResponseDto>> GetAllAsync();
        Task<BookingResponseDto> UpdateStatusAsync(int bookingId, BookingStatus status);
        Task<BookingResponseDto> RebookAsync(int bookingId, int userId);
    }
}
