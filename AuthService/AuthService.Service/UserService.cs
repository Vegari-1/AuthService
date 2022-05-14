using AuthService.Model;
using AuthService.Repository.Interface;
using AuthService.Service.Interface;
using AuthService.Service.Interface.Exceptions;

namespace AuthService.Service;

public class UserService : IUserService
{

    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User> Register(User user)
    {
        User existingUser = await _userRepository.GetByEmailOrUsername(user.Email, user.Username);

        if (existingUser != null)
        {
            throw new EntityExistsException(typeof(User), "email or username");
        }

        return await _userRepository.Save(user);
    }
}

