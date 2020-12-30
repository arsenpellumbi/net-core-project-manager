using ProjectManager.Core.Domain;
using ProjectManager.Core.SeedWork.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectManager.Infrastructure.Persistence.PostgreSQL.Repositories
{
    public sealed class ProjectRepository : IProjectRepository
    {
        private readonly ProjectManagerDbContext _dbContext;

        public IUnitOfWork UnitOfWork => _dbContext;

        public ProjectRepository(ProjectManagerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(Project project)
        {
            _dbContext.Projects.Add(project);
        }

        public void Update(Project project)
        {
            _dbContext.Projects.Update(project);
        }

        public void Delete(Project project)
        {
            _dbContext.Projects.Remove(project);
        }

        public async Task<Project> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _dbContext.Projects
                .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<bool> Exists(string name, CancellationToken cancellationToken)
        {
            var project = await _dbContext.Projects
                .SingleOrDefaultAsync(x => x.Title.ToLower() == name.ToLower(), cancellationToken);

            return project != null;
        }
    }
}