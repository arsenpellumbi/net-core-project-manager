using System;

namespace ProjectManager.Queries.ProjectManagement
{
    public sealed class GetProjectById : IQuery<GetProjectByIdResult>
    {
        public Guid Id { get; set; }
    }

    public sealed class GetProjectByIdResult
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
    }
}