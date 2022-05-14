using AuthService.Dto;
using AuthService.Model;
using AuthService.Service.Interface;
using AuthService.Service.Interface.Exceptions;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace AuthService.UnitTests;

public class UserControllerTests
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
    private static RegisterRequest registerRequest;
    private static RegisterResponse registerResponse;

    private static Mock<IUserService> mockService = new Mock<IUserService>();
    private static Mock<IMapper> mockMapper = new Mock<IMapper>();

    UserController userController = new UserController(mockService.Object, mockMapper.Object);

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

        registerRequest = new RegisterRequest()
        {
            Username = username,
            Email = email,
            Password = password,
            Name = name,
            Surname = surname
        };

        registerResponse = new RegisterResponse()
        {
            Id = id,
            Username = username,
            Email = email,
            Name = name,
            Surname = surname
        };

    }

    [Fact]
    public async void Register_CorrectData_RegistrationResponse()
    {
        SetUp();

        mockService
            .Setup(service => service.Register(user))
            .ReturnsAsync(savedUser);

        mockMapper
           .Setup(x => x.Map<User>(registerRequest))
           .Returns((RegisterRequest source) =>
           {
               return user;
           });

        mockMapper
           .Setup(x => x.Map<RegisterResponse>(savedUser))
           .Returns((User source) =>
           {
               return registerResponse;
           });


        var response = await userController.Register(registerRequest);

        var actionResult = Assert.IsType<ObjectResult>(response);
        var actionValue = Assert.IsType<RegisterResponse>(actionResult.Value);
        Assert.Equal(registerResponse.Id, actionValue.Id);
        Assert.Equal(registerResponse.Username, actionValue.Username);
        Assert.Equal(registerResponse.Email, actionValue.Email);
        Assert.Equal(registerResponse.Name, actionValue.Name);
        Assert.Equal(registerResponse.Surname, actionValue.Surname);
    }

    [Fact]
    public async void Register_ExistingUsername_EntityExistsException()
    {
        SetUp();

        var exception = new EntityExistsException(typeof(User), "email or username");

        mockService
            .Setup(service => service.Register(user))
            .ThrowsAsync(exception);

        mockMapper
           .Setup(x => x.Map<User>(registerRequest))
           .Returns((RegisterRequest source) =>
           {
               return user;
           });

        try
        {
            var response = await userController.Register(registerRequest);
        }
        catch (Exception ex)
        {
            var thrownException = Assert.IsType<EntityExistsException>(ex);
        }
    }
}
