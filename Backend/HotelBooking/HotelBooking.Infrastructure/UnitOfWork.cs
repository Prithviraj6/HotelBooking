using HotelBooking.Domain.Interfaces;
using HotelBooking.Domain.Interfaces.Repositories;
using HotelBooking.Infrastructure.Data;
namespace HotelBooking.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public IUserRepository Users { get; private set; }
        public IHotelRepository Hotels { get; private set; }
        public IRoomRepository Rooms { get; private set; }
        public IBookingRepository Bookings { get; private set; }
        public IPaymentRepository Payments { get; private set; }
        public IReviewRepository Reviews { get; private set; }
        public IPromotionRepository Promotions { get; private set; }

        public IRoomTypeRepository RoomTypes { get; private set; }

        public UnitOfWork(AppDbContext context,
            IUserRepository users,
            IHotelRepository hotels,
            IRoomRepository rooms,
            IBookingRepository bookings,
            IPaymentRepository payments,
            IReviewRepository reviews,
            IPromotionRepository promotions,
            IRoomTypeRepository roomTypes)
        {
            _context = context;
            Users = users;
            Hotels = hotels;
            Rooms = rooms;
            Bookings = bookings;
            Payments = payments;
            Reviews = reviews;
            Promotions = promotions;
            RoomTypes = roomTypes;
        }

        public async Task<int> SaveChangesAsync()
            => await _context.SaveChangesAsync();

        public void Dispose()
            => _context.Dispose();
    }
}
