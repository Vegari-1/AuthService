using AuthService.Model;

namespace AuthService.Service.Interface;

public interface IUserService
{
    Task<User> Register(User user);
}

