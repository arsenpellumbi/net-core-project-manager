using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ProjectManager.Core.SeedWork.Domain;
using ProjectManager.Core.SeedWork.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ProjectManager.Infrastructure.Persistence.PostgreSQL.Services
{
    public class PaginationService : IPaginationService
    {
        private readonly ITypeAdapter _typeAdapter;
        private readonly PaginationOptions _options;

        public PaginationService(ITypeAdapter typeAdapter, PaginationOptions options)
        {
            _typeAdapter = typeAdapter;
            _options = options;
        }

        public async Task<PaginatedItemsResult<TItemResult>> PaginateAsync<TItemResult>(IQueryable<TItemResult> source, int pageIndex, int pageSize,
            CancellationToken cancellationToken = default) where TItemResult : class
        {
            if (pageIndex < 0)
            {
                pageIndex = 0;
            }

            pageSize = pageSize == 0 ? _options.DefaultPageSize : Math.Min(pageSize, _options.MaxPageSizeAllowed);

            var count = await source.LongCountAsync(cancellationToken);

            var data = await source.Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PaginatedItemsResult<TItemResult>
            (
                data,
                (int)Math.Ceiling((decimal)count / pageSize),
                count
            );
        }

        public async Task<PaginatedItemsResult<TItemResult>> PaginateAsync<TItemResult>
            (
                IQueryable<BaseEntity> source,
                int pageIndex,
                int pageSize,
                CancellationToken cancellationToken = default
            ) where TItemResult : class
        {
            var paginatedResult = await PaginateAsync(source, pageIndex, pageSize, cancellationToken);

            return new PaginatedItemsResult<TItemResult>
                (
                    _typeAdapter.Adapt<IEnumerable<TItemResult>>(paginatedResult.Data),
                    paginatedResult.TotalPages,
                    paginatedResult.Count
                );
        }
    }

    public class PaginationOptions
    {
        /// <summary>
        /// Default is 20
        /// </summary>
        public int MaxPageSizeAllowed { get; set; } = 20;

        public int DefaultPageSize { get; set; } = 20;
    }
}