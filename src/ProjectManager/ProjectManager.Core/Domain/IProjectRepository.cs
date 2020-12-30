using ProjectManager.Core.SeedWork.Domain;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectManager.Core.Domain
{
    public interface IProjectRepository : IRepository<Project>
    {
        void Add(Project project);

        void Update(Project project);

        void Delete(Project project);

        Task<Project> GetByIdAsync(Guid id, CancellationToken cancellationToken);

        Task<bool> Exists(string name, CancellationToken cancellationToken);
    }
}