using ProjectManager.Core.Domain;
using ProjectManager.Infrastructure.Persistence.PostgreSQL.Common;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace ProjectManager.Infrastructure.Persistence.PostgreSQL.EntityConfigurations
{
    internal class ConfigProject : EfEntityTypeConfiguration<Project>
    {
        public override void PostConfigure(EntityTypeBuilder<Project> builder)
        {
            builder.Property(x => x.Title)
                .HasMaxLength(100)
                .IsRequired();
            builder.Property(x => x.Description)
                .HasMaxLength(1000)
                .IsRequired();
        }
    }
}