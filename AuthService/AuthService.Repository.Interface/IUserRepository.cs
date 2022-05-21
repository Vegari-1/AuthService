using AuthService.Model;

namespace AuthService.Repository.Interface
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetByUsername(string username);

        Task<User> GetByEmailOrUsername(string email, string username);
    }
}

