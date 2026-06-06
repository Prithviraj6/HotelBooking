using AutoMapper;
using HotelBooking.Application.DTOs.Auth;
using HotelBooking.Application.DTOs.Booking;
using HotelBooking.Application.DTOs.Hotel;
using HotelBooking.Application.DTOs.Payment;
using HotelBooking.Application.DTOs.Promotion;
using HotelBooking.Application.DTOs.Review;
using HotelBooking.Application.DTOs.Room;
using HotelBooking.Application.DTOs.RoomType;
using HotelBooking.Domain.Entities;

namespace HotelBooking.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // ── User → AuthResponseDto ────────────────────────────────────────
            // Token & TokenExpiry are set manually in AuthService after mapping.
            CreateMap<User, AuthResponseDto>()
                .ForMember(d => d.Role, opt => opt.MapFrom(s => s.Role.ToString()))
                .ForMember(d => d.Token, opt => opt.Ignore())
                .ForMember(d => d.TokenExpiry, opt => opt.Ignore());

            // ── Hotel → HotelResponseDto ──────────────────────────────────────
            // AverageRating & TotalReviews are computed from the Reviews collection.
            CreateMap<Hotel, HotelResponseDto>()
                .ForMember(d => d.AverageRating, opt => opt.MapFrom(s =>
                    s.Reviews != null && s.Reviews.Any(r => !r.IsDeleted)
                        ? Math.Round(s.Reviews.Where(r => !r.IsDeleted).Average(r => r.Rating), 1)
                        : 0.0))
                .ForMember(d => d.TotalReviews, opt => opt.MapFrom(s =>
                    s.Reviews != null ? s.Reviews.Count(r => !r.IsDeleted) : 0));

            // ── RoomType → RoomTypeResponseDto ───────────────────────────────
            CreateMap<RoomType, RoomTypeResponseDto>()
                .ForMember(d => d.HotelName, opt => opt.MapFrom(s =>
                    s.Hotel != null ? s.Hotel.Name : null))
                .ForMember(d => d.Category, opt => opt.MapFrom(s => s.Category.ToString()));

            // ── Room → RoomResponseDto ────────────────────────────────────────
            // Pricing & type info comes from the eagerly-loaded RoomType nav prop.
            CreateMap<Room, RoomResponseDto>()
                .ForMember(d => d.HotelName, opt => opt.MapFrom(s =>
                    s.Hotel != null ? s.Hotel.Name : null))
                .ForMember(d => d.RoomTypeName, opt => opt.MapFrom(s =>
                    s.RoomType != null ? s.RoomType.TypeName : null))
                .ForMember(d => d.Category, opt => opt.MapFrom(s =>
                    s.RoomType != null ? s.RoomType.Category.ToString() : null))
                .ForMember(d => d.PricePerNight, opt => opt.MapFrom(s =>
                    s.RoomType != null ? s.RoomType.PricePerNight : 0m))
                .ForMember(d => d.MaxOccupancy, opt => opt.MapFrom(s =>
                    s.RoomType != null ? s.RoomType.MaxOccupancy : 0))
                .ForMember(d => d.Amenities, opt => opt.MapFrom(s =>
                    s.RoomType != null ? s.RoomType.Amenities : null))
                .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()));

            // ── Booking → BookingResponseDto ──────────────────────────────────
            // Navigation: Room.Hotel, Room.RoomType, User must be eagerly loaded.
            CreateMap<Booking, BookingResponseDto>()
                .ForMember(d => d.UserName, opt => opt.MapFrom(s =>
                    s.User != null ? $"{s.User.FirstName} {s.User.LastName}" : null))
                .ForMember(d => d.UserEmail, opt => opt.MapFrom(s =>
                    s.User != null ? s.User.Email : null))
                .ForMember(d => d.RoomNumber, opt => opt.MapFrom(s =>
                    s.Room != null ? s.Room.RoomNumber : null))
                .ForMember(d => d.RoomType, opt => opt.MapFrom(s =>
                    s.Room != null && s.Room.RoomType != null ? s.Room.RoomType.TypeName : null))
                .ForMember(d => d.HotelName, opt => opt.MapFrom(s =>
                    s.Room != null && s.Room.Hotel != null ? s.Room.Hotel.Name : null))
                .ForMember(d => d.HotelCity, opt => opt.MapFrom(s =>
                    s.Room != null && s.Room.Hotel != null ? s.Room.Hotel.City : null))
                .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()));

            // ── Payment → PaymentResponseDto ──────────────────────────────────
            // Navigation: Booking.User, Booking.Room.Hotel must be eagerly loaded.
            CreateMap<Payment, PaymentResponseDto>()
                .ForMember(d => d.UserName, opt => opt.MapFrom(s =>
                    s.Booking != null && s.Booking.User != null
                        ? $"{s.Booking.User.FirstName} {s.Booking.User.LastName}"
                        : null))
                .ForMember(d => d.HotelName, opt => opt.MapFrom(s =>
                    s.Booking != null && s.Booking.Room != null && s.Booking.Room.Hotel != null
                        ? s.Booking.Room.Hotel.Name
                        : null))
                .ForMember(d => d.Method, opt => opt.MapFrom(s => s.Method.ToString()))
                .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()));

            // ── Review → ReviewResponseDto ────────────────────────────────────
            CreateMap<Review, ReviewResponseDto>()
                .ForMember(d => d.UserName, opt => opt.MapFrom(s =>
                    s.User != null ? $"{s.User.FirstName} {s.User.LastName}" : null))
                .ForMember(d => d.HotelName, opt => opt.MapFrom(s =>
                    s.Hotel != null ? s.Hotel.Name : null));

            // ── Promotion → PromotionResponseDto ─────────────────────────────
            CreateMap<Promotion, PromotionResponseDto>()
                .ForMember(d => d.HotelName, opt => opt.MapFrom(s =>
                    s.Hotel != null ? s.Hotel.Name : null));
        }
    }
}
