using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ProjectManager.Core.SeedWork.Domain;
using ProjectManager.Core.SeedWork.Interfaces;
using ProjectManager.Infrastructure.Persistence.PostgreSQL;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ProjectManager.Queries.ProjectManagement.Handlers
{
    public sealed class GetTaskByIdHandler : IRequestHandler<GetTaskById, GetTaskByIdResult>
    {
        private readonly ProjectManagerDbContext _dbContext;
        private readonly ITypeAdapter _typeAdapter;

        public GetTaskByIdHandler(ProjectManagerDbContext dbContext, ITypeAdapter typeAdapter)
        {
            _dbContext = dbContext;
            _typeAdapter = typeAdapter;
        }

        public async Task<GetTaskByIdResult> Handle(GetTaskById request, CancellationToken cancellationToken)
        {
            var projectTask = await _dbContext.Tasks
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (projectTask == null)
            {
                throw new NotFoundException("Task does not exist!");
            }

            return _typeAdapter.Adapt<GetTaskByIdResult>(projectTask);
        }
    }
}