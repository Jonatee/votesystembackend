using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using votesystembackend.Domain.Entities;
using votesystembackend.Infrastructure.Persistence;

namespace votesystembackend.Infrastructure.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(Guid id);
        Task AddAsync(User user);
    }

    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _db;

        public UserRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(User user)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
        }

        public Task<User?> GetByEmailAsync(string email)
            => _db.Users.FirstOrDefaultAsync(x => x.Email == email);

        public Task<User?> GetByIdAsync(Guid id)
            => _db.Users.FirstOrDefaultAsync(x => x.Id == id);
    }
}
