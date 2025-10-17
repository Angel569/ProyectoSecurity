using Galaxy.Security.Domain.OutPort.Secrets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using VaultSharp;

namespace Galaxy.Security.Infraestructure.Adapters.Secrets
{
    public class VaultSecretsProvider : IVaultSecretsProvider
    {
        private readonly IVaultClient _vaultClient;
        private readonly string _secretPath;
        private readonly string _mountPoint;

        public VaultSecretsProvider(IVaultClient vaultClient, string secretPath, string mountPoint)
        {
             _vaultClient = vaultClient;
            _secretPath = secretPath;
            _mountPoint = mountPoint;
        }

        public async Task<Dictionary<string, string>> GetSecretsAsync()
        {
            var secret = await _vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(
                path: _secretPath,
                mountPoint: _mountPoint
            );

            var Data = secret.Data.Data;
            var dict = new Dictionary<string, string>();

            foreach (var item in Data)
            {
                dict[item.Key] = item.Value.ToString() ?? string.Empty;
            }
            return dict;
        }

        public async Task<string?> GetSecretAsync(string key)
        {
            var secrets = await GetSecretsAsync();
            secrets.TryGetValue(key, out var value);
            return value;
        }
    }
}
