using Galaxy.Security.Domain.Dpo;
using Galaxy.Security.Domain.Entities;
using Galaxy.Security.Domain.OutPort.Persistence;
using Galaxy.Security.Infraestructure.Configurations.IdentityEntities;
using Mapster;
using Microsoft.AspNetCore.Identity;


namespace Galaxy.Security.Infraestructure.Adapters.Persistence
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<UserExtension> _userManager;
        public UserRepository(UserManager<UserExtension> userManager) 
        {
            _userManager = userManager;
        }

        public async Task<OperationResult> CreateUserAsync(User user)
        {
            var result = await _userManager.CreateAsync(user.Adapt<UserExtension>(), user.Password);
            return result.Succeeded ? OperationResult.Ok() 
                : OperationResult.Fail(result.Errors.Select(e => e.Description));
        }
    }
}
