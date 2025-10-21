using Galaxy.Security.Domain.OutPort.Persistence;
using Galaxy.Security.Domain.OutPort.Secrets;
using Galaxy.Security.Domain.OutPort.Services;
using Galaxy.Security.Infraestructure.Adapters.Persistence;
using Galaxy.Security.Infraestructure.Adapters.Services;
using Galaxy.Security.Infraestructure.Configurations.Context;
using Galaxy.Security.Infraestructure.Configurations.IdentityEntities;
using Galaxy.Security.Infraestructure.Configurations.Secrets;
using Galaxy.Security.Shared.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Galaxy.Security.Infraestructure
{
    public static class DependecyInjection
    {
        public static IServiceCollection AddInfraestructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Integrar Vault
            services.AddVaultSecrets(configuration);

            var sp = services.BuildServiceProvider();
            var secretProvider = sp.GetRequiredService<IVaultSecretsProvider>();
            var secrets = secretProvider.GetSecretsAsync().GetAwaiter().GetResult(); // sincronizar

            // Inyección del contexto de base de datos
            services.AddDbContext<IdentityDbContext>((sp, options) =>
            {
                var cs = secrets["SecurityDb"];
                options.UseNpgsql(cs);
            });
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var redisConnection = secrets["RedisConnection"];
                return ConnectionMultiplexer.Connect(redisConnection);
            });
            //ASPNET Core Identity Injection
            services.AddIdentity<UserExtension, IdentityRole>(policy =>
            {
                policy.Password.RequiredLength = SecurityConstants.MinLengthPassword;
                policy.Password.RequireDigit = true;
                policy.Password.RequireLowercase = true;
                policy.Password.RequireUppercase = true;
                policy.Password.RequireNonAlphanumeric = false;
                policy.User.RequireUniqueEmail = true;
                policy.SignIn.RequireConfirmedEmail = true;
                policy.Lockout.MaxFailedAccessAttempts = SecurityConstants.MaxFailedAccessAttempts;
                policy.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(SecurityConstants.LockoutTimeSpan);
                policy.Lockout.AllowedForNewUsers = true;
            })
                .AddEntityFrameworkStores<IdentityDbContext>()
                .AddDefaultTokenProviders();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IRefreshTokenStore, RefreshTokenStore>();
            return services;
        }
    }
}
