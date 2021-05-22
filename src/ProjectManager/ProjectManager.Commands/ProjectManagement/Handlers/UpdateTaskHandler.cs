using ProjectManager.Core.Domain;
using ProjectManager.Core.SeedWork.Domain;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace ProjectManager.Commands.ProjectManagement.Handlers
{
    public sealed class UpdateTaskHandler : IRequestHandler<UpdateTask, Unit>
    {
        private readonly IProjectTaskRepository _projectTaskRepository;
        private readonly ILogger<UpdateTaskHandler> _logger;

        public UpdateTaskHandler(IProjectTaskRepository projectTaskRepository, ILogger<UpdateTaskHandler> logger)
        {
            _projectTaskRepository = projectTaskRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(UpdateTask request, CancellationToken cancellationToken)
        {
            var projectTask = await _projectTaskRepository.GetByIdAsync(request.Id, cancellationToken);
            if (projectTask == null)
            {
                throw new DomainException("Task does not exist!");
            }

            if (projectTask.Title != request.Title && await _projectTaskRepository.Exists(request.Title, projectTask.ProjectId, cancellationToken))
            {
                throw new DomainException($"Task with name {request.Title} exists!");
            }

            projectTask.Title = request.Title;
            projectTask.Description = request.Description;
            projectTask.Type = request.Type;
            projectTask.ModifiedDate = DateTime.UtcNow;

            _logger.LogInformation("----- Updating Task - Task: {@Task}", projectTask);

            _projectTaskRepository.Update(projectTask);

            await _projectTaskRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}