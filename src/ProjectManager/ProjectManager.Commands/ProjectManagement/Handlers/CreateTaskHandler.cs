using ProjectManager.Core.Domain;
using ProjectManager.Core.SeedWork.Domain;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectManager.Commands.ProjectManagement.Handlers
{
    public sealed class CreateTaskHandler : IRequestHandler<CreateTask, Guid>
    {
        private readonly IProjectTaskRepository _projectTaskRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IIdGenerator _idGenerator;
        private readonly ILogger<CreateTaskHandler> _logger;

        public CreateTaskHandler(IProjectTaskRepository projectTaskRepository,
            IProjectRepository projectRepository,
            IIdGenerator idGenerator,
            ILogger<CreateTaskHandler> logger)
        {
            _projectTaskRepository = projectTaskRepository;
            _projectRepository = projectRepository;
            _idGenerator = idGenerator;
            _logger = logger;
        }

        public async Task<Guid> Handle(CreateTask request, CancellationToken cancellationToken)
        {
            if (await _projectRepository.GetByIdAsync(request.ProjectId, cancellationToken) == null)
            {
                throw new DomainException("Project does not exist!");
            }

            if (await _projectTaskRepository.Exists(request.Title, request.ProjectId, cancellationToken))
            {
                throw new DomainException($"Task with name {request.Title} exists!");
            }

            var projectTask = new ProjectTask
            {
                Id = _idGenerator.NewId(),
                Title = request.Title,
                Description = request.Description,
                Type = request.Type,
                ProjectId = request.ProjectId
            };

            _logger.LogInformation("----- Creating Task - Task: {@Task}", projectTask);

            _projectTaskRepository.Add(projectTask);

            await _projectTaskRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return projectTask.Id;
        }
    }
}