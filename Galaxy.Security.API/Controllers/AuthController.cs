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
        public AuthController(ICreateUserUseCase createUserUseCase)
        {
            _createUserUseCase = createUserUseCase;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] CreateUserRequest request)
        {
            var result = await _createUserUseCase.ExecuteAsync(request );
            return Ok(BaseResponse<IdentityResponse>.Success(result));
        }
    }
}
