using AuthService.Model;

namespace AuthService.Service.Interface;

public interface IUserService
{
    Task<string> Login(User user);

    Task<User> Register(User user);
}

