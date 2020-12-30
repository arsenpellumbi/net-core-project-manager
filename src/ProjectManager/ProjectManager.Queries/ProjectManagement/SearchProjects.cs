using ProjectManager.Infrastructure.Persistence.PostgreSQL.Services;
using System;

namespace ProjectManager.Queries.ProjectManagement
{
    public sealed class SearchProjects : IQuery<PaginatedItemsResult<SearchProjectsItemResult>>
    {
        public string Value { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }

    public sealed class SearchProjectsItemResult
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
    }
}