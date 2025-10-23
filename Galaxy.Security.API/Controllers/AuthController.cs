using Galaxy.Security.Application.Dto;
using Galaxy.Security.Application.Dto.Users;
using Galaxy.Security.Application.InPorts.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Galaxy.Security.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ICreateUserUseCase _createUserUseCase;
        private readonly ILoginUseCase _loginUseCase;
        private readonly IRefreshTokenUseCase _refreshTokenUseCase;
        private readonly IRemoveCookiesUseCase _removeCookiesUseCase;
        public AuthController(ICreateUserUseCase createUserUseCase, ILoginUseCase loginUseCase, IRefreshTokenUseCase refreshTokenUseCase, IRemoveCookiesUseCase removeCookiesUseCase)
        {
            _createUserUseCase = createUserUseCase;
            _loginUseCase = loginUseCase;
            _refreshTokenUseCase = refreshTokenUseCase;
            _removeCookiesUseCase = removeCookiesUseCase;
        }

        [HttpPost("CreateUser")]
        [Authorize]
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
        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            var result = _removeCookiesUseCase.ExecuteAsync();
            return Ok(BaseResponse<string>.Success(result));
        }
        [HttpPost("Refresh")]
        public async Task<IActionResult> Refresh()
        {
            await _refreshTokenUseCase.ExecuteAsync();
            return Ok(BaseResponse<string>.Success("Token refrescado"));
        }
        [HttpGet("Me")]
        [Authorize]
        public IActionResult GetUserInfo()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if(identity == null || !identity.IsAuthenticated) 
                return Unauthorized();

            var claims = identity.Claims.Select(c => new { c.Type, c.Value });
            return Ok(BaseResponse<object>.Success(claims));
        }
    }
}
