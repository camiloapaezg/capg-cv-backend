using capg_hv_backend.Domain.Entities;
using FluentValidation;

namespace capg_hv_backend.Application.Validators.Internal;

public class FormalEducationAddValidator : AbstractValidator<FormalEducation>
{
    public FormalEducationAddValidator()
    {
        RuleFor(x => x.Degree).NotNull().NotEmpty();
        RuleFor(x => x.School).NotNull().NotEmpty();
        RuleFor(x => x.StartDate).NotNull().NotEmpty();
        RuleFor(x => x.Id).Equal(Guid.Empty);
    }
}

public class FormalEducationUpdateValidator : AbstractValidator<FormalEducation>
{
    public FormalEducationUpdateValidator()
    {
        RuleFor(x => x.Degree).NotNull().NotEmpty();
        RuleFor(x => x.School).NotNull().NotEmpty();
        RuleFor(x => x.StartDate).NotNull().NotEmpty();
        RuleFor(x => x.Id).NotNull().NotEmpty();
    }
}