using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectManager.Core.SeedWork.Domain
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}