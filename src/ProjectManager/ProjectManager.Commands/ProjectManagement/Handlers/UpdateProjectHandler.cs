using System;
using System.Threading;
using System.Threading.Tasks;
using ProjectManager.Core.Domain;
using ProjectManager.Core.SeedWork.Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ProjectManager.Commands.ProjectManagement.Handlers
{
    public sealed class UpdateProjectHandler : IRequestHandler<UpdateProject, Unit>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly ILogger<UpdateProjectHandler> _logger;

        public UpdateProjectHandler(IProjectRepository projectRepository, ILogger<UpdateProjectHandler> logger)
        {
            _projectRepository = projectRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(UpdateProject request, CancellationToken cancellationToken)
        {
            var project = await _projectRepository.GetByIdAsync(request.Id, cancellationToken);
            if (project == null)
            {
                throw new DomainException($"Project does not exist!");
            }

            if (project.Title != request.Title && await _projectRepository.Exists(request.Title, cancellationToken))
            {
                throw new DomainException($"Project with name {request.Title} exists!");
            }

            project.Title = request.Title;
            project.Description = request.Description;

            _logger.LogInformation("----- Updating Project - Project: {@Project}", project);

            _projectRepository.Update(project);

            await _projectRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}