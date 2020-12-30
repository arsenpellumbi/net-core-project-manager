using FluentValidation;

namespace ProjectManager.Commands.ProjectManagement.Validators
{
    public sealed class UpdateTaskValidator : AbstractValidator<UpdateTask>
    {
        public UpdateTaskValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Task id is not valid!");
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Task name is not valid!");
        }
    }
}