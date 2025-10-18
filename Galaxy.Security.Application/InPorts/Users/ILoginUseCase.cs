
using Galaxy.Security.Application.Dto.Users;

namespace Galaxy.Security.Application.InPorts.Users
{
    public interface ILoginUseCase
    {
        Task<string> ExecuteAsync(LoginRequest request);
    }
}
