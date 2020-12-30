using ProjectManager.Infrastructure.Persistence.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace ProjectManager.API.Infrastructure.Persistence
{
    public class ProjectManagerDbContextFactory : IDesignTimeDbContextFactory<ProjectManagerDbContext>
    {
        public ProjectManagerDbContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                           .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
                           .AddJsonFile("appsettings.json")
                           .AddEnvironmentVariables()
                           .Build();
            var optionsBuilder = new DbContextOptionsBuilder<ProjectManagerDbContext>();

            optionsBuilder.UseNpgsql(
                config["ConnectionString"],
                o => o.MigrationsAssembly(typeof(ProjectManagerDbContext).Assembly.GetName().Name)
            );

            return new ProjectManagerDbContext(optionsBuilder.Options);
        }
    }
}