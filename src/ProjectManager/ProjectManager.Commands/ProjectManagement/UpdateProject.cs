using System;
using MediatR;

namespace ProjectManager.Commands.ProjectManagement
{
    public sealed class UpdateProject : ICommand<Unit>
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}