using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Galaxy.Security.Infraestructure.Configurations.Context
{
    public class IdentityDbContextFactory : IDesignTimeDbContextFactory<IdentityDbContext>
    {
        public IdentityDbContext CreateDbContext(string[] args)
        {
            var devConnectionString = "Host=localhost;Port=1500;Database=security_db;Username=admin;Password=Password2025";
            var connectionString = Environment.GetEnvironmentVariable("SecurityDb") ?? devConnectionString;

            var optionsBuilder = new DbContextOptionsBuilder<IdentityDbContext>();
            optionsBuilder.UseNpgsql(connectionString,
                opt => opt.MigrationsHistoryTable("__EFMigrationHistory"));

            return new IdentityDbContext(optionsBuilder.Options);
        }
    }
}
