using ProjectManager.Core.Domain;
using ProjectManager.Core.SeedWork.Domain;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectManager.Commands.ProjectManagement.Handlers
{
    public sealed class CreateProjectHandler : IRequestHandler<CreateProject, Guid>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IIdGenerator _idGenerator;
        private readonly ILogger<CreateProjectHandler> _logger;

        public CreateProjectHandler(IProjectRepository projectRepository,
            IIdGenerator idGenerator,
            ILogger<CreateProjectHandler> logger)
        {
            _projectRepository = projectRepository;
            _idGenerator = idGenerator;
            _logger = logger;
        }

        public async Task<Guid> Handle(CreateProject request, CancellationToken cancellationToken)
        {
            if (await _projectRepository.Exists(request.Title, cancellationToken))
            {
                throw new DomainException($"Project with name {request.Title} exists!");
            }

            var project = new Project
            {
                Id = _idGenerator.NewId(),
                Title = request.Title,
                Description = request.Description,
                CreatedDate = DateTime.UtcNow
            };

            _logger.LogInformation("----- Creating Project - Project: {@Project}",
                project);

            _projectRepository.Add(project);

            await _projectRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return project.Id;
        }
    }
}