using ProjectManager.Infrastructure.Persistence.PostgreSQL;
using ProjectManager.Infrastructure.Persistence.PostgreSQL.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectManager.Queries.ProjectManagement.Handlers
{
    public sealed class SearchTasksInProjectHandler
        : IRequestHandler<SearchTasksInProject, PaginatedItemsResult<SearchTasksInProjectItemResult>>
    {
        private readonly ProjectManagerDbContext _dbContext;
        private readonly IPaginationService _paginationService;

        public SearchTasksInProjectHandler(ProjectManagerDbContext dbContext,

            IPaginationService paginationService)
        {
            _dbContext = dbContext;
            _paginationService = paginationService;
        }

        public async Task<PaginatedItemsResult<SearchTasksInProjectItemResult>> Handle(SearchTasksInProject request,
            CancellationToken cancellationToken)
        {
            var projectTasks = _dbContext.Tasks
                .AsNoTracking()
                .Where(x => x.ProjectId == request.ProjectId && (x.Title.ToLower().Contains(request.Value.ToLower()) || x.Description.ToLower().Contains(request.Value.ToLower())))
                .OrderBy(x => x.Title.Length)
                .ThenByDescending(x => x.CreatedDate);

            var result = await _paginationService.PaginateAsync<SearchTasksInProjectItemResult>(projectTasks, request.PageIndex,
                request.PageSize, cancellationToken);

            return result;
        }
    }
}