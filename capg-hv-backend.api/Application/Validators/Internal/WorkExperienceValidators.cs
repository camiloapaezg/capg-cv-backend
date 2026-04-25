using capg_hv_backend.Domain.Entities;
using FluentValidation;

namespace capg_hv_backend.Application.Validators.Internal;

public class WorkExperienceAddValidator : AbstractValidator<WorkExperience>
{
    public WorkExperienceAddValidator()
    {
        RuleFor(x => x.JobTitle).NotNull().NotEmpty();
        RuleFor(x => x.Company).NotNull().NotEmpty();
        RuleFor(x => x.Description).NotNull().NotEmpty();
        RuleFor(x => x.Until)
            .LessThan(x => x.Until)
            .When(x => x.Until is not null);
        RuleFor(x => x.From)
            .NotNull()
            .NotEmpty()
            .LessThan(DateTime.UtcNow)
            .LessThan(x => x.Until)
            .When(x => x.Until is not null);
        RuleFor(x => x.Id).Equal(Guid.Empty);
    }
}

public class WorkExperienceUpdateValidator : AbstractValidator<WorkExperience>
{
    public WorkExperienceUpdateValidator()
    {
        RuleFor(x => x.JobTitle).NotNull().NotEmpty();
        RuleFor(x => x.Company).NotNull().NotEmpty();
        RuleFor(x => x.Description).NotNull().NotEmpty();
        RuleFor(x => x.Until)
            .LessThan(x => x.Until)
            .When(x => x.Until is not null);
        RuleFor(x => x.From)
            .NotNull()
            .NotEmpty()
            .LessThan(DateTime.UtcNow)
            .LessThan(x => x.Until)
            .When(x => x.Until is not null);
        RuleFor(x => x.Id).NotNull().NotEmpty();
    }
}