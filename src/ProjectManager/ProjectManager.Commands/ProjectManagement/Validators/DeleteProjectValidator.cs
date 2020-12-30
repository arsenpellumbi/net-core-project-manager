using FluentValidation;

namespace ProjectManager.Commands.ProjectManagement.Validators
{
    public sealed class DeleteProjectValidator : AbstractValidator<DeleteProject>
    {
        public DeleteProjectValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Project id is not valid!");
        }
    }
}