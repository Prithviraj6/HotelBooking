
namespace HotelBooking.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendRegistrationConfirmationAsync(string toEmail, string userName);
        Task SendBookingConfirmationAsync(string toEmail, string userName, int bookingId,
            string hotelName, DateTime checkIn, DateTime checkOut, decimal totalPrice);
        Task SendBookingCancellationAsync(string toEmail, string userName, int bookingId);
        Task SendPaymentReceiptAsync(string toEmail, string userName,
            decimal amount, string transactionId);
        Task SendCheckInReminderAsync(string toEmail, string userName,
            string hotelName, DateTime checkIn);
    }
}
