using ProjectManager.Core;
using ProjectManager.Core.SeedWork.Domain;
using ProjectManager.Core.SeedWork.Interfaces;
using ProjectManager.Infrastructure.Persistence.PostgreSQL;
using Autofac;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectManager.API.Infrastructure.AutofacModules
{
    public class ApplicationModule
        : Module
    {
        private readonly IConfiguration _configuration;

        public ApplicationModule(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<IdGenerator>()
                .As<IIdGenerator>()
                .SingleInstance();

            builder.Register(x => new ProjectManagerFileProvider(AppContext.BaseDirectory))
                .As<IProjectManagerFileProvider>()
                .SingleInstance();

            builder.RegisterType<AppTypeFinder>()
                .As<ITypeFinder>()
                .SingleInstance();

            builder.RegisterDbContext<ProjectManagerDbContext>(optionsBuilder =>
                optionsBuilder.UseNpgsql(_configuration["ConnectionString"], npgOptions =>
                {
                    npgOptions.MigrationsAssembly(typeof(ProjectManagerDbContext).Assembly.GetName().Name);
                    npgOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorCodesToAdd: new List<string>());
                })
            );
            builder.AddNpgSqlDbExceptionParser(x =>
            {
                x.CombinationUniqueErrorTemplate = ErrorMessages.FieldsCombinationUniqueErrorTemplate;
                x.UniqueErrorTemplate = ErrorMessages.FieldUniqueErrorTemplate;
            });

            AddAutoMapperProfiles(builder);

            builder.RegisterAssemblyTypes(typeof(ProjectManagerDbContext).Assembly)
                .AsClosedTypesOf(typeof(IRepository<>))
                .InstancePerLifetimeScope();
        }

        private static void AddAutoMapperProfiles(ContainerBuilder builder)
        {
            //find mapper configurations provided by other assemblies

            builder.RegisterType<AutoMapperTypeAdapter>()
                .As<ITypeAdapter>()
                .SingleInstance();

            builder.Register(x =>
            {
                var typeFinder = x.Resolve<ITypeFinder>();

                var mapperConfigurations = typeFinder.FindClassesOfType<IMapperProfile>();

                //create and sort instances of mapper configurations
                var instances = mapperConfigurations
                    .Select(mapperConfiguration => (IMapperProfile)Activator.CreateInstance(mapperConfiguration))
                    .OrderBy(mapperConfiguration => mapperConfiguration.Order);

                //create AutoMapper configuration
                var config = new MapperConfiguration(cfg =>
                {
                    foreach (var instance in instances)
                    {
                        cfg.AddProfile(instance.GetType());
                    }
                });

                return config.CreateMapper();
            }).As<IMapper>()
            .SingleInstance();
        }
    }
}