using capg_hv_backend.Domain.Entities;
using FluentValidation;

namespace capg_hv_backend.Application.Validators.Internal;

public class CertificationTrainingAddValidator : AbstractValidator<CertificationTraining>
{
    public CertificationTrainingAddValidator()
    {
        RuleFor(x => x.Title).NotNull().NotEmpty();
        RuleFor(x => x.Institution).NotNull().NotEmpty();
        RuleFor(x => x.FinishedAt).NotNull().NotEmpty();
        RuleFor(x => x.Id).Equal(Guid.Empty);
    }
}

public class CertificationTrainingUpdateValidator : AbstractValidator<CertificationTraining>
{
    public CertificationTrainingUpdateValidator()
    {
        RuleFor(x => x.Title).NotNull().NotEmpty();
        RuleFor(x => x.Institution).NotNull().NotEmpty();
        RuleFor(x => x.FinishedAt).NotNull().NotEmpty();
        RuleFor(x => x.Id).NotNull().NotEmpty();
    }
}