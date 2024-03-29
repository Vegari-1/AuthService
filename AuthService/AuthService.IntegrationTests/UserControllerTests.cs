using AuthService.Dto;
using AuthService.Model;
using AuthService.Repository;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AuthService.IntegrationTests
{
    public class UserControllerTests : IClassFixture<IntegrationWebApplicationFactory<Program, AppDbContext>>
    {
        private readonly IntegrationWebApplicationFactory<Program, AppDbContext> _factory;
        private readonly HttpClient _client;

        public UserControllerTests(IntegrationWebApplicationFactory<Program, AppDbContext> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        private static readonly string schemaName = "auth";
        private static readonly string tableName = "Users";
        private static readonly Guid id = Guid.NewGuid();
        private static readonly string username = "username";
        private static readonly string email = "email@example.com";
        private static readonly string role = "ROLE_USER";
        private static readonly string password = "password";
        private static readonly string hashPassword = "$2a$12$vp4wrXirrV1vvY34f2QFleupB9NEFpXrrGTeIN6PiATfmMqh6uGTy";

        [Fact]
        public async Task Register_CorrectData_ApiException()
        {
            // Given
            RegisterRequest registerRequest = new RegisterRequest()
            {
                Username = username,
                Email = email,
                Password = password,
                Name = "",
                Surname = "",
                Gender = "",
                DateOfBirth = DateTime.Now,
                Phone = "",
                Public = true,
                Picture = "",
                Biography = ""
            };

            var requestContent = new StringContent(JsonConvert.SerializeObject(registerRequest), Encoding.UTF8, "application/json");

            // When
            var response = await _client.PostAsync("/api/user/register", requestContent);

            // Then
            Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);

            /*response.EnsureSuccessStatusCode();
            var responseContentString = await response.Content.ReadAsStringAsync();
            var responseContentObject = JsonConvert.DeserializeObject<RegisterResponse>(responseContentString);
            Assert.NotNull(responseContentObject);
            Assert.Equal(username, responseContentObject.Username);
            Assert.Equal(email, responseContentObject.Email);
            Assert.Equal(1L, _factory.CountTableRows(schemaName, tableName));

            // Rollback
            _factory.DeleteById(schemaName, tableName, responseContentObject.Id);*/
        }

        [Fact]
        public async void Login_CorrectData_AccessToken()
        {
            // Given
            User user = new User()
            {
                Id = id,
                Username = username,
                Email = email,
                Password = hashPassword,
                Role = role
            };
            _factory.Insert(schemaName, tableName, user);

            LoginRequest loginRequest = new LoginRequest()
            {
                Username = username,
                Password = password
            };
            var requestContent = new StringContent(JsonConvert.SerializeObject(loginRequest), Encoding.UTF8, "application/json");

            // When
            var response = await _client.PostAsync("/api/user/login", requestContent);

            // Then
            response.EnsureSuccessStatusCode();
            var responseContentString = await response.Content.ReadAsStringAsync();
            Assert.NotNull(responseContentString);

            // Rollback
            _factory.DeleteById(schemaName, tableName, id);
        }

        [Fact]
        public async void Login_NonExistingUsername_BadCredentialsException()
        {
            // Given
            LoginRequest loginRequest = new LoginRequest()
            {
                Username = username,
                Password = password
            };
            var requestContent = new StringContent(JsonConvert.SerializeObject(loginRequest), Encoding.UTF8, "application/json");

            // When
            var response = await _client.PostAsync("/api/user/login", requestContent);

            // Then
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
