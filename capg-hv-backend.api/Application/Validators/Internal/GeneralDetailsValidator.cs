using capg_hv_backend.Endpoints.Entities;
using FluentValidation;

namespace capg_hv_backend.Application.Validators.Internal;

public class GeneralDetailsValidator : AbstractValidator<GeneralDetailsDto>
{
    public GeneralDetailsValidator()
    {
        RuleFor(x => x.Title).NotNull().NotEmpty();
    }
}