using Galaxy.Security.Application.InPorts.Users;
using Galaxy.Security.Application.UseCases.Users;
using Microsoft.Extensions.DependencyInjection;

namespace Galaxy.Security.Application
{
    public static class DependecyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<ICreateUserUseCase, CreateUserUseCase>();
            services.AddScoped<ILoginUseCase, LoginUseCase>();
            //services.AddScoped<IRefreshTokenUseCase, RefreshTokenUseCase>();
            //services.AddScoped<IRemoveCookiesUseCase, RemoveCookiesUseCase>();
            return services;
        }
    }
}
