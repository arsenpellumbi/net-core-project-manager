using ProjectManager.Infrastructure.Persistence.PostgreSQL;
using ProjectManager.Infrastructure.Persistence.PostgreSQL.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectManager.Queries.ProjectManagement.Handlers
{
    public sealed class SearchProjectsHandler
        : IRequestHandler<SearchProjects, PaginatedItemsResult<SearchProjectsItemResult>>
    {
        private readonly ProjectManagerDbContext _dbContext;
        private readonly IPaginationService _paginationService;

        public SearchProjectsHandler(ProjectManagerDbContext dbContext,
            IPaginationService paginationService)
        {
            _dbContext = dbContext;
            _paginationService = paginationService;
        }

        public async Task<PaginatedItemsResult<SearchProjectsItemResult>> Handle(SearchProjects request,
            CancellationToken cancellationToken)
        {
            var projects = _dbContext.Projects
                .AsNoTracking()
                .Where(x => x.Title.ToLower().Contains(request.Value.ToLower()) ||  x.Description.ToLower().Contains(request.Value.ToLower()))
                .OrderByDescending(x => x.CreatedDate);

            var result = await _paginationService.PaginateAsync<SearchProjectsItemResult>(projects, request.PageIndex,
                request.PageSize, cancellationToken);

            return result;
        }
    }
}