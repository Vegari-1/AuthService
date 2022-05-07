using AuthService.Model;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Repository
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> opt ) : base(opt)
        {

        }

        public DbSet<User> Users { get; set; }
    }
}

