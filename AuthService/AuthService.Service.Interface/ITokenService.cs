using AuthService.Model;

namespace AuthService.Service.Interface
{
	public interface ITokenService
	{
		string GenerateAccessToken(User user);
	}
}

