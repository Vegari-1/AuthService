using AuthService.Model;

namespace AuthService.Service.Interface
{
    public interface IProfileService
    {
        Task<Profile> CreateProfile(Profile profile);
    }
}
