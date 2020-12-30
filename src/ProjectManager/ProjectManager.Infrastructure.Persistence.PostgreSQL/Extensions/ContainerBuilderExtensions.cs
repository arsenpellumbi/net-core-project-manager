using Microsoft.EntityFrameworkCore;
using System;
using ProjectManager.Infrastructure.Persistence.PostgreSQL.Services;

// ReSharper disable once CheckNamespace
namespace Autofac
{
    public static class ContainerBuilderExtensions
    {
        public static void RegisterDbContext<TContext>(
            this ContainerBuilder builder,
            Action<DbContextOptionsBuilder<TContext>> dbContextOptionsBuilder)
            where TContext : DbContext
        {
            var optionsBuilder = new DbContextOptionsBuilder<TContext>();
            dbContextOptionsBuilder.Invoke(optionsBuilder);

            builder.Register(componentContext => optionsBuilder.Options)
                .As<DbContextOptions<TContext>>()
                .InstancePerLifetimeScope();

            builder.RegisterType<TContext>()
                .AsSelf()
                .InstancePerLifetimeScope();
        }

        public static void AddNpgSqlDbExceptionParser(this ContainerBuilder builder, Action<MessageTemplatesConfig> options)
        {
            var @instance = new MessageTemplatesConfig();

            options.Invoke(instance);

            builder.Register(x => new NpgSqlDbExceptionParserProvider(instance))
                .As<IDbExceptionParserProvider>()
                .SingleInstance();
        }
    }
}