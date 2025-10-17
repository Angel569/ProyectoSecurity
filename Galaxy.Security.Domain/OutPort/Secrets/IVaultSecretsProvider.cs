using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galaxy.Security.Domain.OutPort.Secrets
{
    public interface IVaultSecretsProvider
    {
        Task<Dictionary<string, string>> GetSecretsAsync();
        Task<string?> GetSecretAsync(string key);
    }
}
