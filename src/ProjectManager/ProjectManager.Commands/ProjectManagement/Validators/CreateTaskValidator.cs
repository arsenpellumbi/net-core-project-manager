using FluentValidation;

namespace ProjectManager.Commands.ProjectManagement.Validators
{
    public sealed class CreateTaskValidator : AbstractValidator<CreateTask>
    {
        public CreateTaskValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Name is not valid!");

            RuleFor(x => x.ProjectId)
                .NotEmpty()
                .WithMessage("Project id is not valid!");
        }
    }
}