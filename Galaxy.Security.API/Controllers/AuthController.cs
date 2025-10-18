using Galaxy.Security.Application.Dto;
using Galaxy.Security.Application.Dto.Users;
using Galaxy.Security.Application.InPorts.Users;
using Microsoft.AspNetCore.Mvc;

namespace Galaxy.Security.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ICreateUserUseCase _createUserUseCase;
        private readonly ILoginUseCase _loginUseCase;
        public AuthController(ICreateUserUseCase createUserUseCase, ILoginUseCase loginUseCase)
        {
            _createUserUseCase = createUserUseCase;
            _loginUseCase = loginUseCase;
        }

        [HttpPost("CreateUser")]
        public async Task<IActionResult> Register([FromBody] CreateUserRequest request)
        {
            var result = await _createUserUseCase.ExecuteAsync(request);
            return Ok(BaseResponse<IdentityResponse>.Success(result));
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _loginUseCase.ExecuteAsync(request);
            return Ok(BaseResponse<string>.Success(result));
        }
    }
}
