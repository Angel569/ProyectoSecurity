
using Galaxy.Security.Domain.Dpo;

namespace Galaxy.Security.Domain.OutPort.Services
{
    public interface IRefreshTokenStore
    {
        Task SaveTokenAsync(string userId, string refreshToken, TimeSpan expiration);
        Task<RefreshTokenDpo?> GetTokenAsync(string refreshToken);
        Task InvalidateTokenAsync(string refreshToken);
        Task<string> GetUserIdFromTokenAsync(string refreshToken);
    }
}
