using Galaxy.Security.Domain.OutPort.Secrets;
using Galaxy.Security.Infraestructure.Adapters.Secrets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaultSharp;
using VaultSharp.V1.AuthMethods.Token;

namespace Galaxy.Security.Infraestructure.Configurations.Secrets
{
    public static class VaultConfigurationExtension
    {
        public static IServiceCollection AddVaultSecrets(this IServiceCollection services, IConfiguration configuration)
        {
            var vaultConfig = configuration.GetSection("Vault");
            var vaultAddress = vaultConfig["Address"];
            var vaultToken = vaultConfig["Token"];
            var secretsPath = vaultConfig["SecretsPath"];
            var mountPoint = vaultConfig["MountPoint"];

            // crear cliente de Vault
            var authMethod = new TokenAuthMethodInfo(vaultToken);
            var vaultClientSettings = new VaultClientSettings(vaultAddress, authMethod);
            IVaultClient vaultClient = new VaultClient(vaultClientSettings);

            // puedes ponerlo como singleton
            services.AddSingleton(vaultClient);

            // Registrar un servicio que obtenga secretos cuando se necesite
            services.AddScoped<IVaultSecretsProvider>(sp =>
                new VaultSecretsProvider(vaultClient, secretsPath, mountPoint));

            return services;
        }
    }
}
