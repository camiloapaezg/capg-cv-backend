using capg_hv_backend.Domain.Entities;
using FluentValidation;

namespace capg_hv_backend.Application.Validators.Internal;

public class PersonalDetailsAddValidator : AbstractValidator<PersonalDetails>
{
    public PersonalDetailsAddValidator()
    {
        RuleFor(x => x.BirthDate).LessThan(DateTime.UtcNow);
        RuleFor(x => x.TelephoneNumber)
            .Must(phone => phone.All(c => char.IsDigit(c) || c == '+' || c == '-'))
            .WithMessage("Phone number contains invalid characters.");
        RuleFor(x => x.Id).Equal(Guid.Empty);
    }
}

public class PersonalDetailsUpdateValidator : AbstractValidator<PersonalDetails>
{
    public PersonalDetailsUpdateValidator()
    {
        RuleFor(x => x.BirthDate).LessThan(DateTime.UtcNow);
        RuleFor(x => x.TelephoneNumber)
            .Must(phone => phone.All(c => char.IsDigit(c) || c == '+' || c == '-'))
            .WithMessage("Phone number contains invalid characters.");
        RuleFor(x => x.Id).NotNull().NotEmpty();
    }
}