using capg_hv_backend.Endpoints.Entities;
using FluentValidation;

namespace capg_hv_backend.Application.Validators.Internal;

public class FormalEducationValidator : AbstractValidator<FormalEducationDto>
{
    public FormalEducationValidator()
    {
        RuleFor(x => x.Degree).NotNull().NotEmpty();
        RuleFor(x => x.Institution).NotNull().NotEmpty();
        RuleFor(x => x.StartDate).NotNull().NotEmpty();
    }
}