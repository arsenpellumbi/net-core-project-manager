using System;

namespace ProjectManager.Commands.ProjectManagement
{
    public sealed class CreateProject : ICommand<Guid>
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }
}