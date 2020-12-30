using ProjectManager.Infrastructure.Persistence.PostgreSQL;
using ProjectManager.Infrastructure.Persistence.PostgreSQL.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectManager.Queries.ProjectManagement.Handlers
{
    public sealed class GetTasksByProjectIdHandler : IRequestHandler<GetTasksByProjectId, PaginatedItemsResult<GetTasksByProjectIdItemResult>>
    {
        private readonly ProjectManagerDbContext _dbContext;
        private readonly IPaginationService _paginationService;

        public GetTasksByProjectIdHandler(ProjectManagerDbContext dbContext, IPaginationService paginationService)
        {
            _dbContext = dbContext;
            _paginationService = paginationService;
        }

        public async Task<PaginatedItemsResult<GetTasksByProjectIdItemResult>> Handle(GetTasksByProjectId request, CancellationToken cancellationToken)
        {
            var projectTasks = _dbContext.Tasks.AsNoTracking()
                .Where(x => x.ProjectId == request.ProjectId)
                .OrderByDescending(x => x.CreatedDate);

            var result = await _paginationService.PaginateAsync<GetTasksByProjectIdItemResult>(projectTasks, request.PageIndex,
                request.PageSize, cancellationToken);

            return result;
        }
    }
}