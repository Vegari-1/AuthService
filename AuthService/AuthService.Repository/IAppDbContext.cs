using AuthService.Model;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Repository
{
	public interface IAppDbContext
	{
		DbSet<User> Users { get; set; }
	}
}

