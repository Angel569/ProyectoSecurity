using Galaxy.Security.Application.InPorts.Users;

namespace Galaxy.Security.API.Middlewares
{
    public class RefreshTokenMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceProvider _serviceProvider;
        public RefreshTokenMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
        {
            _next = next;
            _serviceProvider = serviceProvider;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            using var scope = _serviceProvider.CreateScope();
            var _refreshUseCase = scope.ServiceProvider.GetRequiredService<IRefreshTokenUseCase>();
            
            if (context.Request.Path.StartsWithSegments("/api/auth/login") ||
                context.Request.Path.StartsWithSegments("/api/auth/logout") ||
                context.Request.Path.StartsWithSegments("/api/auth/refresh"))
            {
                await _next(context);
                return;
            }

            var accessToken = context.Request.Cookies["access_token"];
            var refreshToken = context.Request.Cookies["refresh_token"];

            if (!string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
            {
                await _next(context);
                return;
            }

            accessToken = await _refreshUseCase.ExecuteAsync();
            context.Request.Headers["Authorization"] = $"Bearer {accessToken}";
            await _next(context);
        }
    }
    public static class RefreshTokenMiddlewareExtensions
    {
        public static IApplicationBuilder UseRefreshTokenMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RefreshTokenMiddleware>();
        }
    }
}
