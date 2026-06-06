using AutoMapper;
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
        private readonly IMapper _mapper;

        public PaymentService(
            IUnitOfWork unitOfWork,
            IEmailService emailService,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _mapper = mapper;
        }

        public async Task<PaymentResponseDto> CreateAsync(int userId, CreatePaymentDto dto)
        {
            var booking = await _unitOfWork.Bookings.GetBookingWithDetailsAsync(dto.BookingId);
            if (booking == null)
                throw new KeyNotFoundException($"Booking with ID {dto.BookingId} not found.");

            if (booking.UserId != userId)
                throw new UnauthorizedAccessException("You are not authorized to pay for this booking.");

            if (booking.Status != BookingStatus.Pending)
                throw new InvalidOperationException(
                    $"Cannot process payment for a {booking.Status} booking.");

            var existingPayment = await _unitOfWork.Payments.GetPaymentByBookingAsync(dto.BookingId);
            if (existingPayment != null && existingPayment.Status == PaymentStatus.Success)
                throw new InvalidOperationException("Payment has already been made for this booking.");

            var payment = new Payment
            {
                BookingId = dto.BookingId,
                Amount = booking.TotalPrice,
                Method = dto.Method,
                Status = PaymentStatus.Success,
                TransactionId = dto.TransactionId,
                PaidAt = DateTime.UtcNow,
                Booking = booking
            };

            await _unitOfWork.Payments.AddAsync(payment);

            // Auto-confirm booking on successful payment
            booking.Status = BookingStatus.Confirmed;
            await _unitOfWork.Bookings.UpdateAsync(booking);
            await _unitOfWork.SaveChangesAsync();

            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            await _emailService.SendPaymentReceiptAsync(
                user.Email, user.FirstName, payment.Amount, payment.TransactionId);

            return _mapper.Map<PaymentResponseDto>(payment);
        }

        public async Task<PaymentResponseDto> GetByBookingAsync(int bookingId, int userId, string role)
        {
            var booking = await _unitOfWork.Bookings.GetBookingWithDetailsAsync(bookingId);
            if (booking == null)
                throw new KeyNotFoundException($"Booking with ID {bookingId} not found.");

            if (role == "Customer" && booking.UserId != userId)
                throw new UnauthorizedAccessException("You are not authorized to view this payment.");

            var payment = await _unitOfWork.Payments.GetPaymentByBookingAsync(bookingId);
            if (payment == null)
                throw new KeyNotFoundException($"No payment found for booking ID {bookingId}.");

            // Ensure Booking nav is attached for AutoMapper
            payment.Booking = booking;

            return _mapper.Map<PaymentResponseDto>(payment);
        }

        public async Task<IEnumerable<PaymentResponseDto>> GetAllAsync()
        {
            var payments = await _unitOfWork.Payments.GetAllPaymentsAsync();
            return _mapper.Map<List<PaymentResponseDto>>(payments);
        }

        public async Task<PaymentResponseDto> RefundAsync(int paymentId)
        {
            var payment = await _unitOfWork.Payments.GetByIdAsync(paymentId);
            if (payment == null)
                throw new KeyNotFoundException($"Payment with ID {paymentId} not found.");

            if (payment.Status != PaymentStatus.Success)
                throw new InvalidOperationException($"Cannot refund a {payment.Status} payment.");

            var booking = await _unitOfWork.Bookings.GetBookingWithDetailsAsync(payment.BookingId);
            if (booking == null)
                throw new KeyNotFoundException("Associated booking not found.");

            if (booking.Status != BookingStatus.Cancelled)
                throw new InvalidOperationException("Refund is only allowed for cancelled bookings.");

            payment.Status = PaymentStatus.Refunded;
            payment.Booking = booking;
            await _unitOfWork.Payments.UpdateAsync(payment);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<PaymentResponseDto>(payment);
        }
    }
}