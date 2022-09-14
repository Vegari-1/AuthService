using AuthService.Model;
using AuthService.Service.Interface;
using AuthService.Service.Interface.Exceptions;
using Newtonsoft.Json;
using System.Text;

namespace AuthService.Service
{
    public class ProfileService : IProfileService
    {
        private static readonly HttpClient _client = new HttpClient();
        private static string profileServiceUrl = Environment.GetEnvironmentVariable("PROFILE_SERVICE_URL");

        public async Task<Profile> CreateProfile(Profile profile)
        {
            if (profileServiceUrl == null)
            {
                profileServiceUrl = "http://localhost:5002";
            }
            var requestContent = new StringContent(JsonConvert.SerializeObject(profile), Encoding.UTF8, "application/json");
            try
            {
                var response = await _client.PostAsync(profileServiceUrl + "/api/profile", requestContent);
                var responseContentString = await response.Content.ReadAsStringAsync();
                var responseContentObject = JsonConvert.DeserializeObject<Profile>(responseContentString);
                return responseContentObject;
            } catch (HttpRequestException ex)
            {
                Console.WriteLine(ex.Message);
                throw new ApiException("ProfileService");
            }
        }
    }
}
