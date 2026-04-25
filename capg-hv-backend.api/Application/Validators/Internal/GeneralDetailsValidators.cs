using capg_hv_backend.Domain.Entities;
using FluentValidation;

namespace capg_hv_backend.Application.Validators.Internal;

public class GeneralDetailsAddValidator : AbstractValidator<GeneralDetails>
{
    public GeneralDetailsAddValidator()
    {
        RuleFor(x => x.Title).NotNull().NotEmpty();
        RuleFor(x => x.Id).Equal(Guid.Empty);
    }
}

public class GeneralDetailsUpdateValidator : AbstractValidator<GeneralDetails>
{
    public GeneralDetailsUpdateValidator()
    {
        RuleFor(x => x.Title).NotNull().NotEmpty();
        RuleFor(x => x.Id).NotNull().NotEmpty();
    }
}