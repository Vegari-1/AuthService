using AuthService.Dto;
using AutoMapper;
namespace AuthService.Profiles
{
	public class ProfileProfile : Profile
	{

        public ProfileProfile()
        {
            CreateMap<RegisterRequest, Model.Profile>();
        }
    }
}

