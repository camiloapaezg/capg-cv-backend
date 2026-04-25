using capg_hv_backend.Domain.Entities;
using FluentValidation;

namespace capg_hv_backend.Application.Validators.Internal;

public class PublicationAddValidator : AbstractValidator<Publication>
{
    public PublicationAddValidator()
    {
        RuleFor(x => x.Title).NotNull().NotEmpty();
        RuleFor(x => x.Type).NotNull().NotEmpty();
        RuleFor(x => x.PublishedAt).NotNull().NotEmpty();
        RuleFor(x => x.Location).NotNull().NotEmpty();
        RuleFor(x => x.Id).Equal(Guid.Empty);
    }
}

public class PublicationUpdateValidator : AbstractValidator<Publication>
{
    public PublicationUpdateValidator()
    {
        RuleFor(x => x.Title).NotNull().NotEmpty();
        RuleFor(x => x.Type).NotNull().NotEmpty();
        RuleFor(x => x.PublishedAt).NotNull().NotEmpty();
        RuleFor(x => x.Location).NotNull().NotEmpty();
        RuleFor(x => x.Id).NotNull().NotEmpty();
    }
}