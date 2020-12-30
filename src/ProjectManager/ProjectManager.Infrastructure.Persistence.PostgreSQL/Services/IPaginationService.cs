using ProjectManager.Core.SeedWork.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectManager.Infrastructure.Persistence.PostgreSQL.Services
{
    public interface IPaginationService
    {
        /// <summary>
        /// Get a IQueryable source and execute apply pagination against it.
        /// It queries DB two times, first time to get the total count of elements
        /// and the second time to get items based on pageSize and PageIndex.
        /// After queries are executed a mapping operation will be executed
        /// </summary>
        /// <typeparam name="TItemResult"></typeparam>
        /// <param name="source">IQueryable where paginated will be performed</param>
        /// <param name="pageIndex">Current page index (0 based)</param>
        /// <param name="pageSize">Number of rows per page</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        Task<PaginatedItemsResult<TItemResult>> PaginateAsync<TItemResult>(IQueryable<TItemResult> source,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default)
            where TItemResult : class;

        /// <summary>
        /// Get a IQueryable source and execute apply pagination against it.
        /// It queries DB two times, first time to get the total count of elements
        /// and the second time to get items based on pageSize and PageIndex.
        /// After queries are executed a mapping operation will be executed
        /// </summary>
        /// <typeparam name="TItemResult"></typeparam>
        /// <param name="source">IQueryable<see cref="BaseEntity"/> where paginated will be performed</param>
        /// <param name="pageIndex">Current page index (0 based)</param>
        /// <param name="pageSize">Number of rows per page</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        Task<PaginatedItemsResult<TItemResult>> PaginateAsync<TItemResult>(IQueryable<BaseEntity> source,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default)
            where TItemResult : class;
    }

    public sealed class PaginatedItemsResult<TItemResult> where TItemResult : class
    {
        public PaginatedItemsResult(IEnumerable<TItemResult> data, int totalPages, long count)
        {
            TotalPages = totalPages;
            Count = count;
            Data = data;
        }

        public int TotalPages { get; }
        public long Count { get; }
        public IEnumerable<TItemResult> Data { get; }
    }
}