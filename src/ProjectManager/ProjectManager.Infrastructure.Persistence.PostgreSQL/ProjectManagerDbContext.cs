using ProjectManager.Core.Domain;
using ProjectManager.Core.Helpers;
using ProjectManager.Core.SeedWork.Domain;
using ProjectManager.Infrastructure.Persistence.PostgreSQL.Common;
using ProjectManager.Infrastructure.Persistence.PostgreSQL.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectManager.Infrastructure.Persistence.PostgreSQL
{
    public class ProjectManagerDbContext : EfObjectContext
    {
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectTask> Tasks { get; set; }

        public ProjectManagerDbContext(DbContextOptions<ProjectManagerDbContext> options)
            : base(options)
        {
        }

        public ProjectManagerDbContext(DbContextOptions<ProjectManagerDbContext> options,
            IDbExceptionParserProvider exceptionParserProvider)
            : base(options, exceptionParserProvider)
        {
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            AddTimestamps();

            return await base.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            // Dispatch Domain Events collection.
            // Choices:
            // A) Right BEFORE committing data (EF SaveChanges) into the DB will make a single transaction including
            // side effects from the domain event handlers which are using the same DbContext with "InstancePerLifetimeScope" or "scoped" lifetime
            // B) Right AFTER committing data (EF SaveChanges) into the DB will make multiple transactions.
            // You will need to handle eventual consistency and compensatory actions in case of failures in any of the Handlers.
            // await _mediator.DispatchDomainEventsAsync(this);
            //TODO fid a better way to handle domain events
            // After executing this line all the changes (from the Command Handler and Domain Event Handlers)
            // performed through the DbContext will be committed
            await base.SaveChangesAsync(cancellationToken);

            return true;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var configurations = ReflectionHelper.GetClassesOfType(
                typeof(IMappingConfiguration),
                GetType().Assembly
                ).Where(t => !t.IsGenericType);

            foreach (var typeConfiguration in configurations)
            {
                var configuration = (IMappingConfiguration)Activator.CreateInstance(typeConfiguration);
                configuration.ApplyConfiguration(modelBuilder);
            }
        }

        private void AddTimestamps()
        {
            ChangeTracker.DetectChanges();

            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity.GetType().GetInterface(typeof(IAggregateRoot).Name) == null)
                {
                    continue;
                }

                if (entry.State == EntityState.Added)
                {
                    ((IAggregateRoot)entry.Entity).CreatedDate = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    ((IAggregateRoot)entry.Entity).ModifiedDate = DateTime.UtcNow;
                }
            }
        }
    }
}