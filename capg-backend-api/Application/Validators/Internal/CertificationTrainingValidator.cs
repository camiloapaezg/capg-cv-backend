using capg_hv_backend.Endpoints.Entities;
using FluentValidation;

namespace capg_hv_backend.Application.Validators.Internal;

public sealed class CertificationTrainingValidator : AbstractValidator<CertificationTrainingDto>
{
    public CertificationTrainingValidator()
    {
        RuleFor(x => x.Title).NotNull().NotEmpty();
        RuleFor(x => x.Institution).NotNull().NotEmpty();
        RuleFor(x => x.FinishedAt).NotNull().NotEmpty();
    }
}