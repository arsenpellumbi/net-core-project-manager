using System;
using ProjectManager.Infrastructure.Persistence.PostgreSQL.Services;

namespace ProjectManager.Queries.ProjectManagement
{
    public sealed class GetProjects : IQuery<PaginatedItemsResult<GetProjectsItemResult>>
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }

    public sealed class GetProjectsItemResult
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
    }
}