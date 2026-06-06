using HotelBooking.Domain.Interfaces.Repositories;

namespace HotelBooking.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IHotelRepository Hotels { get; }
        IRoomRepository Rooms { get; }
        IBookingRepository Bookings { get; }
        IPaymentRepository Payments { get; }
        IReviewRepository Reviews { get; }
        IPromotionRepository Promotions { get; }
        IRoomTypeRepository RoomTypes { get; }
        IDashboardRepository Dashboard { get; }
        Task<int> SaveChangesAsync();
    }
}
