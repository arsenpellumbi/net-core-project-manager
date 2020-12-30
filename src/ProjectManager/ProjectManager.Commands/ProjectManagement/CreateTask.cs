using System;

namespace ProjectManager.Commands.ProjectManagement
{
    public sealed class CreateTask : ICommand<Guid>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int Type { get; set; }
        public Guid ProjectId { get; set; }
    }
}