using AuthService.Model;

namespace AuthService.Repository.Interface
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetByEmailOrUsername(string email, string username);
    }
}

