using Galaxy.Security.Domain.Dpo;
using Galaxy.Security.Domain.OutPort.Services;
using StackExchange.Redis;
using System.Text.Json;

namespace Galaxy.Security.Infraestructure.Adapters.Services
{
   
    public class RefreshTokenStore : IRefreshTokenStore
    {
        private readonly IDatabase _database;
        public RefreshTokenStore(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }

        public async Task SaveTokenAsync(string userId, string refreshToken, TimeSpan expiration)
        {
            var data = new RefreshTokenDpo
            {
                UserId = userId,
                Token = refreshToken,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.Add(expiration)
            };
            var value = JsonSerializer.Serialize(data);
            var key = $"refresh_token:{refreshToken}";

            await _database.StringSetAsync(key, value, expiration);
        }
        public async Task<RefreshTokenDpo?> GetTokenAsync(string refreshToken)
        {
            var key = $"refresh_token:{refreshToken}";
            var value = await _database.StringGetAsync(key);
            if (value.IsNullOrEmpty)
            {
                return null;
            }
            return JsonSerializer.Deserialize<RefreshTokenDpo>(value!);
        }
        public async Task InvalidateTokenAsync(string refreshToken)
        { 
            var key = $"refresh_token:{refreshToken}";
            await _database.KeyDeleteAsync(key);
        }
        public async Task<string> GetUserIdFromTokenAsync(string refreshToken)
        {
            var result = await GetTokenAsync(refreshToken);
            if(result == null)
            {
                throw new InvalidDataException("El token no esta disponible");
            }
            return result.UserId;
        }
    }
}
