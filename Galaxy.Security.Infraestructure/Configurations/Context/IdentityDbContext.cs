using Galaxy.Security.Infraestructure.Configurations.IdentityEntities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Galaxy.Security.Infraestructure.Configurations.Context
{
    public class IdentityDbContext(DbContextOptions<IdentityDbContext> options) : IdentityDbContext<UserExtension>(options)
    {
    }
}
