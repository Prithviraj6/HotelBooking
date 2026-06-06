using HotelBooking.Application.DTOs.Payment;

namespace HotelBooking.Application.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentResponseDto> CreateAsync(int userId, CreatePaymentDto dto);
        Task<PaymentResponseDto> GetByBookingAsync(int bookingId, int userId, string role);
        Task<IEnumerable<PaymentResponseDto>> GetAllAsync();
        Task<PaymentResponseDto> RefundAsync(int paymentId);
    }
}
