using ProjectManager.Core.SeedWork.Domain;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectManager.Core.Domain
{
    public interface IProjectTaskRepository : IRepository<ProjectTask>
    {
        void Add(ProjectTask projectTask);

        void Update(ProjectTask projectTask);

        void Delete(ProjectTask projectTask);

        Task<ProjectTask> GetByIdAsync(Guid id, CancellationToken cancellationToken);

        Task<bool> Exists(string name, Guid projectId, CancellationToken cancellationToken);

        Task<IEnumerable<ProjectTask>> GetTasksByProjectIdAsync(Guid projectId, CancellationToken cancellationToken);
    }
}