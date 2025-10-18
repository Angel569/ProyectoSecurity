
using Galaxy.Security.Domain.Entities;

namespace Galaxy.Security.Domain.OutPort.Services
{
    public interface IAuthService
    {
        Task<(string accessToken, string refreshToken)> GenerateTokensAsync(User userApp);
    }
}
