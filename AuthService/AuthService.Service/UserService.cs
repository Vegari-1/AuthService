using AuthService.Model;
using AuthService.Repository.Interface;
using AuthService.Service.Interface;
using AuthService.Service.Interface.Exceptions;

namespace AuthService.Service;

public class UserService : IUserService
{

    private static readonly string loginException  = "Incorrect username or password";

    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;

    public UserService(IUserRepository userRepository, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    public async Task<string> Login(User user)
    {
        User existingUser = await _userRepository.GetByUsername(user.Username);

        if (existingUser == null)
            throw new BadCredentialsException(loginException);

        if (!BCrypt.Net.BCrypt.Verify(user.Password, existingUser.Password))
            throw new BadCredentialsException(loginException);

        string accessToken = _tokenService.GenerateAccessToken(existingUser);

        return accessToken;
    }

    public async Task<User> Register(User user)
    {
        User existingUser = await _userRepository.GetByEmailOrUsername(user.Email, user.Username);

        if (existingUser != null)
            throw new EntityExistsException(typeof(User), "email or username");

        user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

        return await _userRepository.Save(user);
    }
}

