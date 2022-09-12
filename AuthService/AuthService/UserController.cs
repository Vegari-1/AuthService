using AuthService.Dto;
using AuthService.Model;
using AuthService.Service.Interface;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OpenTracing;
using Prometheus;

namespace AuthService
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly ITracer _tracer;

        Counter counter = Metrics.CreateCounter("auth_service_counter", "auth counter");

        public UserController(IUserService userService, IMapper mapper, ITracer tracer)
        {
            _userService = userService;
            _mapper = mapper;
            _tracer = tracer;
        }

        [HttpGet("test")]
        public IActionResult Test([FromHeader(Name = "profile-id")] Guid id)
        {
            return Ok(id);
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
            User user = await _userService.Register(
                _mapper.Map<User>(registerRequest), 
                _mapper.Map<Model.Profile>(registerRequest));

            return StatusCode(StatusCodes.Status201Created, _mapper.Map<RegisterResponse>(user));
        }
    }
}

