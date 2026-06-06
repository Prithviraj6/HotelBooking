using HotelBooking.Domain.Entities;

namespace HotelBooking.Domain.Interfaces.Repositories
{
    public interface IPaymentRepository : IGenericRepository<Payment>
    {
        Task<Payment> GetPaymentByBookingAsync(int bookingId);
        Task<IEnumerable<Payment>> GetAllPaymentsAsync();
    }
}
