using AuthService.Model;
using AuthService.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Repository
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context) {}

        public async Task<User> GetByUsername(string username)
        {
            return await _context.Users.Where(x => x.Username == username).FirstOrDefaultAsync();
        }

        public async Task<User> GetByEmailOrUsername(string email, string username)
        {
            return await _context.Users.Where(x => x.Email == email || x.Username == username).FirstOrDefaultAsync();
        }
    }
}

