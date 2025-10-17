using Galaxy.Security.Domain.Dpo;
using Galaxy.Security.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galaxy.Security.Domain.OutPort.Persistence
{
    public interface IUserRepository
    {
        Task<OperationResult> CreateUserAsync(User user);
    }
}
