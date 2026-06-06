using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Interfaces.Repositories;
using HotelBooking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Infrastructure.Repositories
{
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository(AppDbContext context) : base(context) { }

        public async Task<Payment> GetPaymentByBookingAsync(int bookingId)
            => await _dbSet
                .Include(p => p.Booking)
                .FirstOrDefaultAsync(p => p.BookingId == bookingId && !p.IsDeleted);

        public async Task<IEnumerable<Payment>> GetAllPaymentsAsync()
            => await _dbSet
                .Include(p => p.Booking)
                    .ThenInclude(b => b.User)
                .Where(p => !p.IsDeleted)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
    }
}
