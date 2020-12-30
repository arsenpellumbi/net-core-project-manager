using ProjectManager.Infrastructure.Persistence.PostgreSQL.Services;
using System;

namespace ProjectManager.Queries.ProjectManagement
{
    public sealed class SearchTasksInProject : IQuery<PaginatedItemsResult<SearchTasksInProjectItemResult>>
    {
        public Guid ProjectId { get; set; }
        public string Value { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }

    public sealed class SearchTasksInProjectItemResult
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Type { get; set; }
        public Guid ProjectId { get; set; }
    }
}