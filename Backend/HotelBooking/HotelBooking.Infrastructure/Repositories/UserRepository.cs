using Microsoft.EntityFrameworkCore;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Interfaces.Repositories;
using HotelBooking.Infrastructure.Data;

namespace HotelBooking.Infrastructure.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context) { }

        public async Task<User> GetByEmailAsync(string email)
            => await _dbSet.FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);

        public async Task<bool> EmailExistsAsync(string email)
            => await _dbSet.AnyAsync(u => u.Email == email && !u.IsDeleted);
    }
}
