using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Galaxy.Security.Infraestructure.Configurations.IdentityEntities
{
    public class UserExtension : IdentityUser
    {
        public Guid UserId { get; set; }

        [StringLength(200)]
        public string FullName { get; set; } = default!;
    }
}
