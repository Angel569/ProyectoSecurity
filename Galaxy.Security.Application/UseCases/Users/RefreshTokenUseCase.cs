using Galaxy.Security.Application.InPorts.Users;
using Galaxy.Security.Domain.OutPort.Services;

namespace Galaxy.Security.Application.UseCases.Users
{
    public class RefreshTokenUseCase : IRefreshTokenUseCase
    {
        private readonly IAuthService _authService;
        public RefreshTokenUseCase(IAuthService authService)
        {
            _authService = authService;
        }
        public async Task<string> ExecuteAsync()
        {
            var result = await _authService.RefreshTokensAsync();
            return result.AccessToken;
        }
    }
}
