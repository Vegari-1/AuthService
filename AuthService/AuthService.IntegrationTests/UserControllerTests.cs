using AuthService.Dto;
using AuthService.Repository;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AuthService.IntegrationTests
{
    public class userControllerTests : IClassFixture<IntegrationTestFactory<Program, AppDbContext>>
    {
        private readonly IntegrationTestFactory<Program, AppDbContext> _factory;

        public userControllerTests(IntegrationTestFactory<Program, AppDbContext> factory) => _factory = factory;

        private static readonly string tableName = "Users";
        private static readonly string username = "username";
        private static readonly string email = "email@example.com";
        private static readonly string password = "password";
        private static readonly string name = "John";
        private static readonly string surname = "Smith";

        [Fact]
        public async Task Register_CorrectData_RegistrationResponse()
        {
            // Arrange
            var client = _factory.CreateClient();

            RegisterRequest registerRequest = new RegisterRequest()
            {
                Username = username,
                Email = email,
                Password = password,
                Name = name,
                Surname = surname
            };

            var requestContent = new StringContent(JsonConvert.SerializeObject(registerRequest), Encoding.UTF8, "application/json");

            // Act
            var response = await client.PostAsync("/api/user/register", requestContent);

            // Assert
            response.EnsureSuccessStatusCode();
            var responseContentString = await response.Content.ReadAsStringAsync();
            var responseContentObject = JsonConvert.DeserializeObject<RegisterResponse>(responseContentString);
            Assert.NotNull(responseContentObject);
            Assert.Equal(username, responseContentObject.Username);
            Assert.Equal(email, responseContentObject.Email);
            Assert.Equal(name, responseContentObject.Name);
            Assert.Equal(surname, responseContentObject.Surname);
            Assert.Equal(1L, _factory.CountTableRows(tableName));

            // Rollback
            _factory.DeleteById(tableName, responseContentObject.Id);
        }

        // dele kontejner i podatke

        // neophodni podaci u bazi za login
        //[Fact]
        public async void Login_CorrectData_AccessToken()
        {
            // Arrange
            var client = _factory.CreateClient();

            LoginRequest loginRequest = new LoginRequest()
            {
                Username = username,
                Password = password
            };

            var requestContent = new StringContent(JsonConvert.SerializeObject(loginRequest), Encoding.UTF8, "application/json");

            // Act
            var response = await client.PostAsync("/api/user/login", requestContent);

            // Assert
            response.EnsureSuccessStatusCode();
            var responseContentString = await response.Content.ReadAsStringAsync();
            var responseContentObject = JsonConvert.DeserializeObject<string>(responseContentString);
            Assert.NotNull(responseContentObject);
        }

        [Fact]
        public async void Login_NonExistingUsername_BadCredentialsException()
        {
            // Arrange
            var client = _factory.CreateClient();

            LoginRequest loginRequest = new LoginRequest()
            {
                Username = username,
                Password = password
            };

            var requestContent = new StringContent(JsonConvert.SerializeObject(loginRequest), Encoding.UTF8, "application/json");

            // Act
            var response = await client.PostAsync("/api/user/login", requestContent);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
