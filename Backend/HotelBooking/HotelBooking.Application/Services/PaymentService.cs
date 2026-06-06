using HotelBooking.Application.DTOs.Payment;
using HotelBooking.Application.Interfaces;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Enums;
using HotelBooking.Domain.Interfaces;

namespace HotelBooking.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;

        public PaymentService(
            IUnitOfWork unitOfWork,
            IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
        }

        public async Task<PaymentResponseDto> CreateAsync(int userId, CreatePaymentDto dto)
        {
            // Check booking exists
            var booking = await _unitOfWork.Bookings
                .GetBookingWithDetailsAsync(dto.BookingId);

            if (booking == null)
                throw new KeyNotFoundException(
                    $"Booking with ID {dto.BookingId} not found.");

            // Check ownership
            if (booking.UserId != userId)
                throw new UnauthorizedAccessException(
                    "You are not authorized to pay for this booking.");

            // Only pending bookings can be paid
            if (booking.Status != BookingStatus.Pending)
                throw new InvalidOperationException(
                    $"Cannot process payment for a {booking.Status} booking.");

            // Check if payment already exists for this booking
            var existingPayment = await _unitOfWork.Payments
                .GetPaymentByBookingAsync(dto.BookingId);

            if (existingPayment != null &&
                existingPayment.Status == PaymentStatus.Success)
                throw new InvalidOperationException(
                    "Payment has already been made for this booking.");

            // Create payment
            var payment = new Payment
            {
                BookingId = dto.BookingId,
                Amount = booking.TotalPrice,
                Method = dto.Method,
                Status = PaymentStatus.Success,
                TransactionId = dto.TransactionId,
                PaidAt = DateTime.UtcNow
            };

            await _unitOfWork.Payments.AddAsync(payment);

            // Auto confirm booking after successful payment
            booking.Status = BookingStatus.Confirmed;
            await _unitOfWork.Bookings.UpdateAsync(booking);

            await _unitOfWork.SaveChangesAsync();

            // Send payment receipt email
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            await _emailService.SendPaymentReceiptAsync(
                user.Email,
                user.FirstName,
                payment.Amount,
                payment.TransactionId);

            return await MapToResponseDtoAsync(payment);
        }

        public async Task<PaymentResponseDto> GetByBookingAsync(
            int bookingId, int userId, string role)
        {
            // Check booking exists
            var booking = await _unitOfWork.Bookings
                .GetBookingWithDetailsAsync(bookingId);

            if (booking == null)
                throw new KeyNotFoundException(
                    $"Booking with ID {bookingId} not found.");

            // Customer can only view their own booking payment
            if (role == "Customer" && booking.UserId != userId)
                throw new UnauthorizedAccessException(
                    "You are not authorized to view this payment.");

            var payment = await _unitOfWork.Payments
                .GetPaymentByBookingAsync(bookingId);

            if (payment == null)
                throw new KeyNotFoundException(
                    $"No payment found for booking ID {bookingId}.");

            return await MapToResponseDtoAsync(payment);
        }

        public async Task<IEnumerable<PaymentResponseDto>> GetAllAsync()
        {
            var payments = await _unitOfWork.Payments.GetAllPaymentsAsync();
            var result = new List<PaymentResponseDto>();

            foreach (var payment in payments)
                result.Add(await MapToResponseDtoAsync(payment));

            return result;
        }

        public async Task<PaymentResponseDto> RefundAsync(int paymentId)
        {
            var payment = await _unitOfWork.Payments.GetByIdAsync(paymentId);
            if (payment == null)
                throw new KeyNotFoundException(
                    $"Payment with ID {paymentId} not found.");

            // Only successful payments can be refunded
            if (payment.Status != PaymentStatus.Success)
                throw new InvalidOperationException(
                    $"Cannot refund a {payment.Status} payment.");

            // Check booking is cancelled
            var booking = await _unitOfWork.Bookings
                .GetBookingWithDetailsAsync(payment.BookingId);

            if (booking == null)
                throw new KeyNotFoundException("Associated booking not found.");

            if (booking.Status != BookingStatus.Cancelled)
                throw new InvalidOperationException(
                    "Refund is only allowed for cancelled bookings.");

            payment.Status = PaymentStatus.Refunded;
            await _unitOfWork.Payments.UpdateAsync(payment);
            await _unitOfWork.SaveChangesAsync();

            return await MapToResponseDtoAsync(payment);
        }

        // ─── Private Mapper ──────────────────────────────────────────────

        private async Task<PaymentResponseDto> MapToResponseDtoAsync(Payment payment)
        {
            var booking = payment.Booking
                ?? await _unitOfWork.Bookings
                    .GetBookingWithDetailsAsync(payment.BookingId);

            var user = booking?.User
                ?? await _unitOfWork.Users.GetByIdAsync(booking?.UserId ?? 0);

            var room = booking?.Room
                ?? await _unitOfWork.Rooms.GetByIdAsync(booking?.RoomId ?? 0);

            var hotel = room?.Hotel
                ?? await _unitOfWork.Hotels.GetByIdAsync(room?.HotelId ?? 0);

            return new PaymentResponseDto
            {
                Id = payment.Id,
                BookingId = payment.BookingId,
                UserName = $"{user?.FirstName} {user?.LastName}",
                HotelName = hotel?.Name,
                Amount = payment.Amount,
                Method = payment.Method.ToString(),
                Status = payment.Status.ToString(),
                TransactionId = payment.TransactionId,
                PaidAt = payment.PaidAt,
                CreatedAt = payment.CreatedAt
            };
        }
    }
}