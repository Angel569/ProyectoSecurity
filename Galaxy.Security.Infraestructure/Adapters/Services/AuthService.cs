using Galaxy.Security.Domain.Entities;
using Galaxy.Security.Domain.OutPort.Secrets;
using Galaxy.Security.Domain.OutPort.Services;
using Galaxy.Security.Infraestructure.Configurations.IdentityEntities;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Galaxy.Security.Infraestructure.Adapters.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<UserExtension> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRefreshTokenStore _refreshTokenStore;
        private readonly IVaultSecretsProvider _vaulSecretProvider;
        public AuthService(IConfiguration configuration, UserManager<UserExtension> userManager, IHttpContextAccessor httpContextAccessor, IRefreshTokenStore refreshTokenStore, IVaultSecretsProvider vaulSecretProvider)
        {
            _configuration = configuration;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _refreshTokenStore = refreshTokenStore;
            _vaulSecretProvider = vaulSecretProvider;
        }
        public async Task<(string accessToken, string refreshToken)> GenerateTokensAsync(User userApp)
        {
            var secrets = _vaulSecretProvider.GetSecretsAsync().GetAwaiter().GetResult();
            var secKey = secrets["JwtSecretKey"];
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secKey));
            var user = userApp.Adapt<UserExtension>();
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(ClaimTypes.NameIdentifier, user.UserName!),
                new Claim(ClaimTypes.Name, user.FullName!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            var tokenOptions = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpirationMinutes"])),
                signingCredentials: new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256)
            );
            var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            var refreshToken = GenerateSecureToken();
            var expiration = TimeSpan.FromHours(Convert.ToDouble(jwtSettings["RefreshTokenExpirationHours"]));

            // Guardar el refresh token en redis
            await _refreshTokenStore.SaveTokenAsync(user.Id, refreshToken, expiration);

            SetAuthCookie(accessToken, refreshToken);
            return (accessToken, refreshToken);
        }

        public async Task<(string AccessToken, string RefreshToken, User? User)> RefreshTokensAsync()
        {
            var context = _httpContextAccessor.HttpContext!;

            var refreshToken = context.Request.Cookies["refresh_token"];

            if (string.IsNullOrEmpty(refreshToken))
                throw new UnauthorizedAccessException("Refresh token no encontrado");

            var userId = await _refreshTokenStore.GetUserIdFromTokenAsync(refreshToken);
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Refresh token inválido o expirado");

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                throw new UnauthorizedAccessException("Usuario no encontrado");

            var usuario = user.Adapt<User>();

            await _refreshTokenStore.InvalidateTokenAsync(refreshToken);

            var tokens = await GenerateTokensAsync(usuario);

            return (tokens.accessToken, tokens.refreshToken, usuario);
        }

        private string GenerateSecureToken()
        {
            var ramdomBytes = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(ramdomBytes);
            }
            return Convert.ToBase64String(ramdomBytes);
        }
        private void SetAuthCookie(string accessToken, string refreshToken)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var context = _httpContextAccessor.HttpContext!;
            var accessCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpirationMinutes"]))
            };
            var refreshCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddHours(Convert.ToDouble(jwtSettings["RefreshTokenExpirationHours"]))
            };
            context.Response.Cookies.Append("access_token", accessToken, accessCookieOptions);
            context.Response.Cookies.Append("refresh_token", refreshToken, refreshCookieOptions);
        }

        public void RemoveAuthCookies()
        {
            var context = _httpContextAccessor.HttpContext!;

            var accessCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(-1)
            };

            var refreshCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(-1)
            };

            context.Response.Cookies.Append("access_token", "", accessCookieOptions);
            context.Response.Cookies.Append("refresh_token", "", refreshCookieOptions);
        }
    }
}
