using System;
using AuthService.Dto;
using AuthService.Model;
using AuthService.Service.Interface;
using AuthService.Service.Interface.Exceptions;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OpenTracing;
using Xunit;

namespace AuthService.UnitTests;

public class UserControllerTests
{

    private static readonly Guid id = Guid.NewGuid();
    private static readonly string username = "username";
    private static readonly string email = "email@example.com";
    private static readonly string password = "password";
    private static readonly string hashPassword = "$2a$12$vp4wrXirrV1vvY34f2QFleupB9NEFpXrrGTeIN6PiATfmMqh6uGTy";
    private static readonly string accessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCIsImN0eSI6IkpXVCJ9.eyJ1c2VybmFtZSI6ImtzZW5rbyIsImV4cCI6MTY1MzEyOTQ1MiwiaXNzIjoidmVnYXJpLTEiLCJhdWQiOiJ2ZWdhcmktMSJ9.CA6pGWnJjopO53m049x1fg5amU0eqHIhDkwDFwVGguc";

    private static User userFromRegisterRequest;
    private static User userFromLoginRequest;
    private static User savedUser;
    private static RegisterRequest registerRequest;
    private static RegisterResponse registerResponse;
    private static LoginRequest loginRequest;

    private static Mock<IUserService> mockService = new Mock<IUserService>();
    private static Mock<IMapper> mockMapper = new Mock<IMapper>();
    private static Mock<ITracer> mockTracer = new Mock<ITracer>();

    UserController userController = new UserController(mockService.Object, mockMapper.Object, mockTracer.Object);

    private static void SetUp()
    {
        userFromRegisterRequest = new User()
        {
            Username = username,
            Email = email,
            Password = password
        };

        userFromLoginRequest = new User()
        {
            Username = username,
            Password = password,
        };

        savedUser = new User()
        {
            Id = id,
            Username = username,
            Email = email,
            Password = hashPassword
        };

        registerRequest = new RegisterRequest()
        {
            Username = username,
            Email = email,
            Password = password
        };

        registerResponse = new RegisterResponse()
        {
            Id = id,
            Username = username,
            Email = email,
        };

        loginRequest = new LoginRequest()
        {
            Username = username,
            Password = password
        };
    }

    [Fact]
    public async void Register_CorrectData_RegistrationResponse()
    {
        SetUp();

        mockService
            .Setup(service => service.Register(userFromRegisterRequest))
            .ReturnsAsync(savedUser);

        mockMapper
           .Setup(x => x.Map<User>(registerRequest))
           .Returns((RegisterRequest source) =>
           {
               return userFromRegisterRequest;
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
    }

    [Fact]
    public async void Register_ExistingUsername_EntityExistsException()
    {
        SetUp();

        var exception = new EntityExistsException(typeof(User), "email or username");

        mockService
            .Setup(service => service.Register(userFromRegisterRequest))
            .ThrowsAsync(exception);

        mockMapper
           .Setup(x => x.Map<User>(registerRequest))
           .Returns((RegisterRequest source) =>
           {
               return userFromRegisterRequest;
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

    [Fact]
    public async void Login_CorrectData_AccessToken()
    {

        SetUp();

        mockService
            .Setup(service => service.Login(userFromLoginRequest))
            .ReturnsAsync(accessToken);

        mockMapper
           .Setup(x => x.Map<User>(loginRequest))
           .Returns((LoginRequest source) =>
           {
               return userFromLoginRequest;
           });

        var response = await userController.Login(loginRequest);

        var actionResult = Assert.IsType<OkObjectResult>(response);
        var actionValue = Assert.IsType<string>(actionResult.Value);
        Assert.Equal(accessToken, actionValue);
    }

    [Fact]
    public async void Login_NonExistingUsername_BadCredentialsException()
    {
        SetUp();

        string exceptionTitle = "Incorrect username or password";
        var exception = new BadCredentialsException(exceptionTitle);

        loginRequest.Username = "incorrect";

        mockService
            .Setup(service => service.Login(userFromLoginRequest))
            .ThrowsAsync(exception);
        mockMapper
           .Setup(x => x.Map<User>(loginRequest))
           .Returns((LoginRequest source) =>
           {
               return userFromLoginRequest;
           });

        try
        {
            var response = await userController.Login(loginRequest);
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

        string exceptionTitle = "Incorrect username or password";
        var exception = new BadCredentialsException(exceptionTitle);

        loginRequest.Password = "incorrect";

        mockService
            .Setup(service => service.Login(userFromLoginRequest))
            .ThrowsAsync(exception);
        mockMapper
           .Setup(x => x.Map<User>(loginRequest))
           .Returns((LoginRequest source) =>
           {
               return userFromLoginRequest;
           });

        try
        {
            var response = await userController.Login(loginRequest);
        }
        catch (Exception ex)
        {
            var thrownException = Assert.IsType<BadCredentialsException>(ex);
            Assert.Equal(exceptionTitle, thrownException.Message);
        }
    }
}
