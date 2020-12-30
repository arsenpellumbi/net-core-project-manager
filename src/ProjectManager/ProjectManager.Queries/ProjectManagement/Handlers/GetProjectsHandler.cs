using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ProjectManager.Infrastructure.Persistence.PostgreSQL;
using ProjectManager.Infrastructure.Persistence.PostgreSQL.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ProjectManager.Queries.ProjectManagement.Handlers
{
    public sealed class GetProjectsHandler : IRequestHandler<GetProjects, PaginatedItemsResult<GetProjectsItemResult>>
    {
        private readonly ProjectManagerDbContext _dbContext;
        private readonly IPaginationService _paginationService;

        public GetProjectsHandler(ProjectManagerDbContext dbContext, IPaginationService paginationService)
        {
            _dbContext = dbContext;
            _paginationService = paginationService;
        }

        public async Task<PaginatedItemsResult<GetProjectsItemResult>> Handle(GetProjects request, CancellationToken cancellationToken)
        {
            var projects = _dbContext.Projects
                            .AsNoTracking()
                            .OrderByDescending(x => x.CreatedDate);

            var result = await _paginationService.PaginateAsync<GetProjectsItemResult>(projects, request.PageIndex,
                request.PageSize, cancellationToken);

            return result;
        }
    }
}