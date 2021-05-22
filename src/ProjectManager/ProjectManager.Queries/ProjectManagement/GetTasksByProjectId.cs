using ProjectManager.Infrastructure.Persistence.PostgreSQL.Services;
using System;

namespace ProjectManager.Queries.ProjectManagement
{
    public sealed class GetTasksByProjectId : IQuery<PaginatedItemsResult<GetTasksByProjectIdItemResult>>
    {
        public Guid ProjectId { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }

    public sealed class GetTasksByProjectIdItemResult
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Type { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public Guid ProjectId { get; set; }
    }
}