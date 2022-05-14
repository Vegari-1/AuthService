using System;
using AuthService.Model;
using AuthService.Repository.Interface;
using AuthService.Service;
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

        private static User user;
        private static User savedUser;

        private static Mock<IUserRepository> mockRepository = new Mock<IUserRepository>();

        UserService userService = new UserService(mockRepository.Object);

        private static void SetUp()
        {
            user = new User()
            {
                Username = username,
                Email = email,
                Password = password,
                Name = name,
                Surname = surname
            };

            savedUser = new User()
            {
                Id = id,
                Username = username,
                Email = email,
                Password = hashPassword,
                Name = name,
                Surname = surname
            };

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
            Assert.Equal(savedUser.Name, response.Name);
            Assert.Equal(savedUser.Surname, response.Surname);
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

