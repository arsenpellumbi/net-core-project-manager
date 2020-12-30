using ProjectManager.Core.Domain;
using MediatR;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectManager.Commands.ProjectManagement.Handlers
{
    public sealed class DeleteTaskHandler : IRequestHandler<DeleteTask, Unit>
    {
        private readonly IProjectTaskRepository _projectTaskRepository;

        public DeleteTaskHandler(IProjectTaskRepository projectTaskRepository)
        {
            _projectTaskRepository = projectTaskRepository;
        }

        public async Task<Unit> Handle(DeleteTask request, CancellationToken cancellationToken)
        {
            var projectTask = await _projectTaskRepository.GetByIdAsync(request.Id, cancellationToken);
            if (projectTask != null)
            {
                _projectTaskRepository.Delete(projectTask);

                await _projectTaskRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            }

            return Unit.Value;
        }
    }
}