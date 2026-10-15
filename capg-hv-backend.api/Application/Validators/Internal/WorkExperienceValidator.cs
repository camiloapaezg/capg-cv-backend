using capg_hv_backend.Endpoints.Entities;
using FluentValidation;

namespace capg_hv_backend.Application.Validators.Internal;

public class WorkExperienceValidator : AbstractValidator<WorkExperienceDto>
{
    public WorkExperienceValidator()
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
    }
}
