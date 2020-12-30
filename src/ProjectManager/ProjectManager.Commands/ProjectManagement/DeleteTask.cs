using MediatR;
using System;

namespace ProjectManager.Commands.ProjectManagement
{
    public sealed class DeleteTask : ICommand<Unit>
    {
        public Guid Id { get; set; }
    }
}