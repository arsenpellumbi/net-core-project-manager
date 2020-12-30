using ProjectManager.Core.SeedWork.Domain;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectManager.Infrastructure.Persistence.PostgreSQL.Common
{
    /// <summary>
    /// Represents base entity mapping configuration
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public abstract class EfEntityTypeConfiguration<TEntity> : IMappingConfiguration, IEntityTypeConfiguration<TEntity>
        where TEntity : BaseEntity
    {
        /// <summary>
        /// Developers should override this method in custom classes in order to apply their custom configuration code
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public abstract void PostConfigure(EntityTypeBuilder<TEntity> builder);

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public void Configure(EntityTypeBuilder<TEntity> builder)
        {
            var tblName = typeof(TEntity).Name;
            var customTableAttribute = typeof(TEntity).GetCustomAttributes(false);
            if (customTableAttribute.Length > 0)
            {
                tblName = ((TableAttribute)customTableAttribute[0]).Name;
            }

            builder.ToTable(tblName.Pluralize());
            builder.HasKey(x => x.Id);

            //add custom configuration
            PostConfigure(builder);

            builder.UseXminAsConcurrencyToken();
        }

        /// <summary>
        /// Apply this mapping configuration
        /// </summary>
        /// <param name="modelBuilder">The builder being used to construct the model for the database context</param>
        public void ApplyConfiguration(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(this);
        }
    }
}