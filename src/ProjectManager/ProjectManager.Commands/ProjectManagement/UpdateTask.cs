using MediatR;
using System;

namespace ProjectManager.Commands.ProjectManagement
{
    public sealed class UpdateTask : ICommand<Unit>
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Type { get; set; }
    }
}