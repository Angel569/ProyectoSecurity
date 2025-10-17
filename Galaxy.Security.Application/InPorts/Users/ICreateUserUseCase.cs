using Galaxy.Security.Application.Dto;
using Galaxy.Security.Application.Dto.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galaxy.Security.Application.InPorts.Users
{
    public interface ICreateUserUseCase
    {
        Task<IdentityResponse> ExecuteAsync(CreateUserRequest request);
    }
}
