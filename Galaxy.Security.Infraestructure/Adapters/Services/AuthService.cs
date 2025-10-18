using Galaxy.Security.Domain.Entities;
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
        public AuthService(IConfiguration configuration, UserManager<UserExtension> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<(string accessToken, string refreshToken)> GenerateTokensAsync(User userApp)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!));
            var user = userApp.Adapt<UserExtension>();
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Name, user.UserName!),
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
            SetAuthCookie(accessToken, refreshToken);
            return (accessToken, refreshToken);
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
            var context = _httpContextAccessor.HttpContext!;
            var accessCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddMinutes(15)
            };
            var refreshCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddHours(12)
            };
            context.Response.Cookies.Append("access_token", accessToken, accessCookieOptions);
            context.Response.Cookies.Append("refresh_token", refreshToken, refreshCookieOptions);
        }
    }
}
