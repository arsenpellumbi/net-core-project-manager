﻿using FluentValidation;
using System.Linq;

namespace ProjectManager.Commands.ProjectManagement.Validators
{
    public sealed class UpdateProjectValidator : AbstractValidator<UpdateProject>
    {
        public UpdateProjectValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Project id is not valid!");
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Project name is not valid!");
            RuleFor(x => x.Title)
                .Must(y => !y.Any(char.IsWhiteSpace))
                .WithMessage("Project name cannot contain white spaces!");
        }
    }
}