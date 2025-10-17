using Galaxy.Security.Application.Dto;
using Galaxy.Security.Application.Dto.Users;
using Galaxy.Security.Application.InPorts.Users;
using Galaxy.Security.Domain.Dpo;
using Galaxy.Security.Domain.Entities;
using Galaxy.Security.Domain.OutPort.Persistence;

namespace Galaxy.Security.Application.UseCases.Users
{
    public class CreateUserUseCase : ICreateUserUseCase
    {
        private readonly IUserRepository _userRepository;
        public CreateUserUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<IdentityResponse> ExecuteAsync(CreateUserRequest request)
        {
            var user = User.Create(new Guid(), request.FullName, request.UserName, request.Email, request.Password);
            var result = await _userRepository.CreateUserAsync(user);

            if (!result.Success)
            {
               throw new ApplicationException(string.Join(", ", result.Errors));
            }
            return new IdentityResponse
            {
                Success = result.Success,
                Errors = result.Errors
            };
        }
    }
}
