using ProjectManager.Core.Domain;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectManager.Commands.ProjectManagement.Handlers
{
    public sealed class DeleteProjectHandler : IRequestHandler<DeleteProject, Unit>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IProjectTaskRepository _projectTaskRepository;

        public DeleteProjectHandler(IProjectRepository projectRepository,
            IProjectTaskRepository projectTaskRepository)
        {
            _projectRepository = projectRepository;
            _projectTaskRepository = projectTaskRepository;
        }

        public async Task<Unit> Handle(DeleteProject request, CancellationToken cancellationToken)
        {
            var project = await _projectRepository.GetByIdAsync(request.Id, cancellationToken);
            if (project != null)
            {

                var projectTasks = await _projectTaskRepository.GetTasksByProjectIdAsync(project.Id, cancellationToken);
                foreach (var projectTask in projectTasks)
                {
                    _projectTaskRepository.Delete(projectTask);
                }

                _projectRepository.Delete(project);

                await _projectRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            }

            return Unit.Value;
        }
    }
}