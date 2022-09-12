using AuthService.Dto;
using AuthService.Model;

namespace AuthService.Profiles
{
	public class UserProfile : AutoMapper.Profile
	{

        public UserProfile()
        {
            // Source -> Target
            CreateMap<RegisterRequest, User>();
            CreateMap<User, RegisterResponse>();
            CreateMap<LoginRequest, User>();
        }
    }
}

