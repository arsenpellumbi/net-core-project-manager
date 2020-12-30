using System;
using MediatR;

namespace ProjectManager.Commands.ProjectManagement
{
    public sealed class DeleteProject : ICommand<Unit>
    {
        public Guid Id { get; set; }
    }
}