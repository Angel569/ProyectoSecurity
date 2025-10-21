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

        public async Task<OperationResult> CreateUserAsync(User user, string rol)
        {
            var userExtension = user.Adapt<UserExtension>();
            var result = await _userManager.CreateAsync(userExtension, user.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(userExtension, rol);
            }
            return result.Succeeded ? OperationResult.Ok() 
                : OperationResult.Fail(result.Errors.Select(e => e.Description));
        }    

        public async Task<User?> GetUserByUserNameAsync(string userName)
        {
            var result = await _userManager.FindByNameAsync(userName);
            return result.Adapt<User>();
        }
        public async Task<bool> CheckPasswordAsync(User user, string password)
        {
            var userExtension = user.Adapt<UserExtension>();
            var result = await _userManager.CheckPasswordAsync(userExtension, password);
            return result;
        }
    }
}
