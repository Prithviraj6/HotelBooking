using HotelBooking.Application.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace HotelBooking.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private async Task SendEmailAsync(string toEmail, string toName,
            string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(
                _configuration["EmailSettings:SenderName"],
                _configuration["EmailSettings:SenderEmail"]));
            email.To.Add(new MailboxAddress(toName, toEmail));
            email.Subject = subject;
            email.Body = new TextPart("html") { Text = body };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(
                _configuration["EmailSettings:SmtpHost"],
                int.Parse(_configuration["EmailSettings:SmtpPort"]),
                SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(
                _configuration["EmailSettings:SenderEmail"],
                _configuration["EmailSettings:Password"]);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }

        public async Task SendRegistrationConfirmationAsync(
            string toEmail, string userName)
        {
            var body = $@"
                <h2>Welcome, {userName}!</h2>
                <p>Your account has been created successfully.</p>
                <p>You can now browse and book hotels.</p>";

            await SendEmailAsync(toEmail, userName,
                "Welcome to Hotel Booking!", body);
        }

        public async Task SendBookingConfirmationAsync(string toEmail,
            string userName, int bookingId, string hotelName,
            DateTime checkIn, DateTime checkOut, decimal totalPrice)
        {
            var body = $@"
                <h2>Booking Confirmed!</h2>
                <p>Dear {userName},</p>
                <p>Your booking has been confirmed.</p>
                <ul>
                    <li><strong>Booking ID:</strong> {bookingId}</li>
                    <li><strong>Hotel:</strong> {hotelName}</li>
                    <li><strong>Check-In:</strong> {checkIn:dd MMM yyyy}</li>
                    <li><strong>Check-Out:</strong> {checkOut:dd MMM yyyy}</li>
                    <li><strong>Total Price:</strong> ₹{totalPrice:N2}</li>
                </ul>";

            await SendEmailAsync(toEmail, userName,
                $"Booking Confirmation #{bookingId}", body);
        }

        public async Task SendBookingCancellationAsync(string toEmail,
            string userName, int bookingId)
        {
            var body = $@"
                <h2>Booking Cancelled</h2>
                <p>Dear {userName},</p>
                <p>Your booking #{bookingId} has been cancelled.</p>
                <p>If you did not request this, please contact support.</p>";

            await SendEmailAsync(toEmail, userName,
                $"Booking Cancellation #{bookingId}", body);
        }

        public async Task SendPaymentReceiptAsync(string toEmail,
            string userName, decimal amount, string transactionId)
        {
            var body = $@"
                <h2>Payment Successful</h2>
                <p>Dear {userName},</p>
                <p>Your payment has been received.</p>
                <ul>
                    <li><strong>Amount:</strong> ₹{amount:N2}</li>
                    <li><strong>Transaction ID:</strong> {transactionId}</li>
                    <li><strong>Date:</strong> {DateTime.UtcNow:dd MMM yyyy HH:mm}</li>
                </ul>";

            await SendEmailAsync(toEmail, userName, "Payment Receipt", body);
        }

        public async Task SendCheckInReminderAsync(string toEmail,
            string userName, string hotelName, DateTime checkIn)
        {
            var body = $@"
                <h2>Check-In Reminder</h2>
                <p>Dear {userName},</p>
                <p>This is a reminder that your check-in at 
                   <strong>{hotelName}</strong> is tomorrow,
                   <strong>{checkIn:dd MMM yyyy}</strong>.</p>
                <p>We look forward to welcoming you!</p>";

            await SendEmailAsync(toEmail, userName,
                $"Check-In Reminder — {hotelName}", body);
        }
    }
}