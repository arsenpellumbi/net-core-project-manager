using ProjectManager.Core.SeedWork.Domain;
using ProjectManager.Infrastructure.Persistence.PostgreSQL;
using CsvHelper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManager.API.Infrastructure.Persistence
{
    /// <summary>
    /// Add seed data to database using ContainerEngineDbContext
    /// we can provide seed data in csv, in application startup if there are no data in tables
    /// csv should be parsed and insert data to db
    /// </summary>
    public class ProjectManagerDbContextSeed
    {
        private const string SetupFolderPath = "Setup";

        public async Task SeedAsync(ProjectManagerDbContext context,
            IWebHostEnvironment env,
            IIdGenerator idGenerator,
            ILogger<ProjectManagerDbContextSeed> logger)
        {
            var policy = CreatePolicy(logger, nameof(ProjectManagerDbContext));

            await policy.ExecuteAsync(async () =>
            {
                await using (context)
                {
                    await SeedData(context, env, idGenerator);
                }
            });
        }

        private static AsyncRetryPolicy CreatePolicy(ILogger<ProjectManagerDbContextSeed> logger,
            string prefix,
            int retries = 3)
        {
            return Policy.Handle<PostgresException>().WaitAndRetryAsync(
                retryCount: retries,
                sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                onRetry: (exception, timeSpan, retry, ctx) =>
                {
                    logger.LogWarning(exception,
                        "[{prefix}] Exception {ExceptionType} with message {Message} detected on attempt {retry} of {retries}",
                        prefix, exception.GetType().Name, exception.Message, retry, retries);
                }
            );
        }

        private static async Task SeedData(ProjectManagerDbContext context,
            IWebHostEnvironment env,
            IIdGenerator idGenerator)
        {
            await context.SaveChangesAsync();
        }
    }
}