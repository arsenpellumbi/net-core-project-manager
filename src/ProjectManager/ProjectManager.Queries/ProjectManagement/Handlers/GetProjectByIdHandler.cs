using ProjectManager.Core.SeedWork.Domain;
using ProjectManager.Core.SeedWork.Interfaces;
using ProjectManager.Infrastructure.Persistence.PostgreSQL;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectManager.Queries.ProjectManagement.Handlers
{
    public sealed class GetProjectByIdHandler : IRequestHandler<GetProjectById, GetProjectByIdResult>
    {
        private readonly ProjectManagerDbContext _dbContext;
        private readonly ITypeAdapter _typeAdapter;

        public GetProjectByIdHandler(ProjectManagerDbContext dbContext, ITypeAdapter typeAdapter)
        {
            _dbContext = dbContext;
            _typeAdapter = typeAdapter;
        }

        public async Task<GetProjectByIdResult> Handle(GetProjectById request, CancellationToken cancellationToken)
        {
            var project = await _dbContext.Projects
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (project == null)
            {
                throw new NotFoundException("Project not found!");
            }

            return _typeAdapter.Adapt<GetProjectByIdResult>(project);
        }
    }
}