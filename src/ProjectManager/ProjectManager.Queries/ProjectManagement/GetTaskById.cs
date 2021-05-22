using System;

namespace ProjectManager.Queries.ProjectManagement
{
    public sealed class GetTaskById : IQuery<GetTaskByIdResult>
    {
        public Guid Id { get; set; }
    }

    public sealed class GetTaskByIdResult
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