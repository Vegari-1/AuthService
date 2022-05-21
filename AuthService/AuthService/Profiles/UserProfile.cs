using AuthService.Dto;
using AuthService.Model;
using AutoMapper;
namespace AuthService.Profiles
{
	public class UserProfile : Profile
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

