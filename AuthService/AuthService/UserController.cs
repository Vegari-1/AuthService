using AuthService.Dto;
using AuthService.Model;
using AuthService.Service.Interface;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AuthService
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UserController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            string accessToken = await _userService.Login(_mapper.Map<User>(loginRequest));

            return Ok(accessToken);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
        {
            User user = await _userService.Register(_mapper.Map<User>(registerRequest));

            return StatusCode(StatusCodes.Status201Created, _mapper.Map<RegisterResponse>(user));
        }
    }
}

