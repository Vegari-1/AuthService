using System;
using AuthService.Model;
using AuthService.Repository.Interface;
using AuthService.Service;
using AuthService.Service.Interface;
using AuthService.Service.Interface.Exceptions;
using Moq;
using Xunit;

namespace AuthService.UnitTests.ServiceTests
{
	public class UserServiceTests
	{
        private static readonly Guid id = Guid.NewGuid();
        private static readonly string username = "username";
        private static readonly string email = "email@example.com";
        private static readonly string password = "password";
        private static readonly string hashPassword = "$2a$12$vp4wrXirrV1vvY34f2QFleupB9NEFpXrrGTeIN6PiATfmMqh6uGTy";
        private static readonly string name = "John";
        private static readonly string surname = "Smith";
        private static readonly string accessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCIsImN0eSI6IkpXVCJ9.eyJ1c2VybmFtZSI6ImtzZW5rbyIsImV4cCI6MTY1MzEyOTQ1MiwiaXNzIjoidmVnYXJpLTEiLCJhdWQiOiJ2ZWdhcmktMSJ9.CA6pGWnJjopO53m049x1fg5amU0eqHIhDkwDFwVGguc";

        private static User user;
        private static User savedUser;
        private static User loginCredentials;

        private static Mock<IUserRepository> mockRepository = new Mock<IUserRepository>();
        private static Mock<ITokenService> mockTokenService = new Mock<ITokenService>();
 
        UserService userService = new UserService(mockRepository.Object, mockTokenService.Object);

        private static void SetUp()
        {
            user = new User()
            {
                Username = username,
                Email = email,
                Password = password
            };

            savedUser = new User()
            {
                Id = id,
                Username = username,
                Email = email,
                Password = hashPassword
            };

            loginCredentials = new User()
            {
                Username = username,
                Password = password
            };

        }

        [Fact]
        public async void Login_CorrectData_AccessToken()
        {
            SetUp();

            mockRepository
                .Setup(repository => repository.GetByUsername(username))
                .ReturnsAsync(savedUser);
            mockTokenService
                .Setup(service => service.GenerateAccessToken(savedUser))
                .Returns(accessToken);

            var response = await userService.Login(loginCredentials);

            Assert.Equal(accessToken, response);
        }

        [Fact]
        public async void Login_IncorrectUsername_BadCredentialsException()
        {
            SetUp();

            loginCredentials.Username = "invalid";
            string exceptionTitle = "Incorrect username or password";

            mockRepository
                .Setup(repository => repository.GetByUsername(username))
                .ReturnsAsync(null as User);

            try
            {
                var thrownException = await userService.Login(loginCredentials);
            }
            catch (Exception ex)
            {
                var thrownException = Assert.IsType<BadCredentialsException>(ex);
                Assert.Equal(exceptionTitle, thrownException.Message);
            }
        }

        [Fact]
        public async void Login_IncorrectPassword_BadCredentialsException()
        {
            SetUp();

            loginCredentials.Password = "invalid";
            string exceptionTitle = "Incorrect username or password";

            mockRepository
                .Setup(repository => repository.GetByUsername(username))
                .ReturnsAsync(savedUser);

            try
            {
                var thrownException = await userService.Login(loginCredentials);
            }
            catch (Exception ex)
            {
                var thrownException = Assert.IsType<BadCredentialsException>(ex);
                Assert.Equal(exceptionTitle, thrownException.Message);
            }
        }

        [Fact]
        public async void Register_CorrectData_Employee()
        {
            SetUp();

            mockRepository
                .Setup(repository => repository.GetByEmailOrUsername(email, username))
                .ReturnsAsync(null as User);
            mockRepository
                .Setup(repository => repository.Save(user))
                .ReturnsAsync(savedUser);

            var response = await userService.Register(user);

            Assert.Equal(savedUser.Id, response.Id);
            Assert.Equal(savedUser.Username, response.Username);
            Assert.Equal(savedUser.Password, response.Password);
        }

        [Fact]
        public async void Register_ExistingUsername_EntityExistsException()
        {
            SetUp();

            mockRepository
                .Setup(repository => repository.GetByEmailOrUsername(email, username))
                .ReturnsAsync(savedUser);

            try
            {
                var response = await userService.Register(user);
            }
            catch (Exception ex)
            {
                var thrownException = Assert.IsType<EntityExistsException>(ex);
            }
        }
    }
}

