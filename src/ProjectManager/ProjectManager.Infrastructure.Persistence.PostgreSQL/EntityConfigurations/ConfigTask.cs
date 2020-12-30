using ProjectManager.Core.Domain;
using ProjectManager.Infrastructure.Persistence.PostgreSQL.Common;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProjectManager.Infrastructure.Persistence.PostgreSQL.EntityConfigurations
{
    internal class ConfigTask : EfEntityTypeConfiguration<ProjectTask>
    {
        public override void PostConfigure(EntityTypeBuilder<ProjectTask> builder)
        {
            builder.Property(x => x.Title)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.Description)
                .HasMaxLength(1000)
                .IsRequired();

            builder.Property(x => x.Type)
                .IsRequired();

            builder.HasOne(x => x.Project)
                .WithMany(x => x.Tasks)
                .HasForeignKey(x => x.ProjectId)
                .IsRequired();
        }
    }
}