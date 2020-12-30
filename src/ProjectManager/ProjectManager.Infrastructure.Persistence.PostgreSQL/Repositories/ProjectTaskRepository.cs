using ProjectManager.Core.Domain;
using ProjectManager.Core.SeedWork.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectManager.Infrastructure.Persistence.PostgreSQL.Repositories
{
    public sealed class ProjectTaskRepository : IProjectTaskRepository
    {
        private readonly ProjectManagerDbContext _dbContext;

        public IUnitOfWork UnitOfWork => _dbContext;

        public ProjectTaskRepository(ProjectManagerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(ProjectTask projectTask)
        {
            _dbContext.Tasks.Add(projectTask);
        }

        public void Update(ProjectTask projectTask)
        {
            _dbContext.Tasks.Update(projectTask);
        }

        public void Delete(ProjectTask projectTask)
        {
            _dbContext.Tasks.Remove(projectTask);
        }

        public async Task<ProjectTask> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _dbContext.Tasks
                .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<bool> Exists(string name, Guid projectId, CancellationToken cancellationToken)
        {
            var image = await _dbContext.Tasks
                .SingleOrDefaultAsync(x => x.Title.ToLower() == name.ToLower() && x.ProjectId == projectId, cancellationToken);

            return image != null;
        }

        public async Task<IEnumerable<ProjectTask>> GetTasksByProjectIdAsync(Guid projectId, CancellationToken cancellationToken)
        {
            return await _dbContext.Tasks
                .Where(x => x.ProjectId == projectId)
                .ToListAsync(cancellationToken);
        }
    }
}