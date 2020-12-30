using FluentValidation;

namespace ProjectManager.Commands.ProjectManagement.Validators
{
    public sealed class DeleteTaskValidator : AbstractValidator<DeleteTask>
    {
        public DeleteTaskValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Task id is not valid!");
        }
    }
}