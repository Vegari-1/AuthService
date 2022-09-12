using AuthService.Model;
using AuthService.Service.Interface;
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
                profileServiceUrl = "http://localhost:5000";
            }
            var requestContent = new StringContent(JsonConvert.SerializeObject(profile), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(profileServiceUrl + "/api/profile", requestContent);
            var responseContentString = await response.Content.ReadAsStringAsync();
            var responseContentObject = JsonConvert.DeserializeObject<Profile>(responseContentString);
            return responseContentObject;
        }
    }
}
