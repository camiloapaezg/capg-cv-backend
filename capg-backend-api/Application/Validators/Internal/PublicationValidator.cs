using capg_hv_backend.Endpoints.Entities;
using FluentValidation;

namespace capg_hv_backend.Application.Validators.Internal;

public class PublicationValidator : AbstractValidator<PublicationDto>
{
    public PublicationValidator()
    {
        RuleFor(x => x.Title).NotNull().NotEmpty();
        RuleFor(x => x.Type).NotNull().NotEmpty();
        RuleFor(x => x.PublishedAt).NotNull().NotEmpty();
        RuleFor(x => x.Location).NotNull().NotEmpty();
    }
}