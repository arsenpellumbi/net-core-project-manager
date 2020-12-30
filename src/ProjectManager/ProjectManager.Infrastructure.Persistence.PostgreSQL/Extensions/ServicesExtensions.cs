using System;
using ProjectManager.Core.SeedWork.Interfaces;
using ProjectManager.Infrastructure.Persistence.PostgreSQL.Services;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddDefaultPagination(this IServiceCollection services, Action<PaginationOptions> option)
        {
            var paginationOptions = new PaginationOptions();
            option.Invoke(paginationOptions);

            services.AddSingleton<IPaginationService>(x =>
                new PaginationService(x.GetRequiredService<ITypeAdapter>(), paginationOptions)
            );
            return services;
        }
    }
}