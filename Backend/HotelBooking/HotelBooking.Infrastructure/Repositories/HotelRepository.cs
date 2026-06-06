using HotelBooking.Domain.Common;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Enums;
using HotelBooking.Domain.Interfaces.Repositories;
using HotelBooking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Infrastructure.Repositories
{
    public class HotelRepository : GenericRepository<Hotel>, IHotelRepository
    {
        public HotelRepository(AppDbContext context) : base(context) { }

        public async Task<(IEnumerable<Hotel> Hotels, int TotalCount)> SearchHotelsAsync(HotelSearchDto searchDto)
        {
            var query = _dbSet
                .Include(h => h.RoomTypes)
                .Include(h => h.Reviews)
                .Where(h => !h.IsDeleted && h.IsActive)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchDto.City))
                query = query.Where(h => h.City.ToLower().Contains(searchDto.City.ToLower()));

            if (!string.IsNullOrWhiteSpace(searchDto.Search))
                query = query.Where(h =>
                    h.Name.ToLower().Contains(searchDto.Search.ToLower()) ||
                    h.Location.ToLower().Contains(searchDto.Search.ToLower()));

            if (searchDto.StarRating.HasValue)
                query = query.Where(h => h.StarRating == searchDto.StarRating.Value);

            if (searchDto.MinPrice.HasValue)
                query = query.Where(h =>
                    h.RoomTypes.Any(rt =>
                        !rt.IsDeleted &&
                        rt.PricePerNight >= searchDto.MinPrice.Value));

            if (searchDto.MaxPrice.HasValue)
                query = query.Where(h =>
                    h.RoomTypes.Any(rt =>
                        !rt.IsDeleted &&
                        rt.PricePerNight <= searchDto.MaxPrice.Value));

            if (searchDto.MaxOccupancy.HasValue)
                query = query.Where(h =>
                    h.RoomTypes.Any(rt =>
                        !rt.IsDeleted &&
                        rt.MaxOccupancy >= searchDto.MaxOccupancy.Value));

            // If dates provided, only return hotels that have available rooms
            if (searchDto.CheckInDate.HasValue && searchDto.CheckOutDate.HasValue)
                query = query.Where(h =>
                    h.Rooms.Any(r =>
                        !r.IsDeleted &&
                        r.Status == RoomStatus.Available &&
                        !r.Bookings.Any(b =>
                            !b.IsDeleted &&
                            b.Status != BookingStatus.Cancelled &&
                            b.CheckInDate < searchDto.CheckOutDate.Value &&
                            b.CheckOutDate > searchDto.CheckInDate.Value)));

            // Sorting
            query = searchDto.SortBy?.ToLower() switch
            {
                "name" => searchDto.SortOrder == "desc"
                    ? query.OrderByDescending(h => h.Name)
                    : query.OrderBy(h => h.Name),

                "rating" => searchDto.SortOrder == "desc"
                    ? query.OrderByDescending(h => h.StarRating)
                    : query.OrderBy(h => h.StarRating),

                "price" => searchDto.SortOrder == "desc"
                    ? query.OrderByDescending(h =>
                        h.RoomTypes.Where(rt => !rt.IsDeleted)
                                   .Min(rt => rt.PricePerNight))
                    : query.OrderBy(h =>
                        h.RoomTypes.Where(rt => !rt.IsDeleted)
                                   .Min(rt => rt.PricePerNight)),

                _ => query.OrderBy(h => h.Name)
            };

            var totalCount = await query.CountAsync();

            var hotels = await query
                .Skip((searchDto.PageNumber - 1) * searchDto.PageSize)
                .Take(searchDto.PageSize)
                .ToListAsync();

            return (hotels, totalCount);
        }

        public async Task<Hotel> GetHotelWithRoomsAsync(int hotelId)
            => await _dbSet
                .Include(h => h.Rooms.Where(r => !r.IsDeleted))
                    .ThenInclude(r => r.RoomType)
                .FirstOrDefaultAsync(h => h.Id == hotelId && !h.IsDeleted);

        public async Task<Hotel> GetHotelWithReviewsAsync(int hotelId)
            => await _dbSet
                .Include(h => h.Reviews.Where(r => !r.IsDeleted))
                    .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(h => h.Id == hotelId && !h.IsDeleted);
    }
}
