using capg_hv_backend.Endpoints.Entities;
using FluentValidation;

namespace capg_hv_backend.Application.Validators.Internal;

public class PersonalDetailsValidator : AbstractValidator<PersonalDetailsDto>
{
    public PersonalDetailsValidator()
    {
        RuleFor(x => x.BirthDate).LessThan(DateTime.UtcNow);
        RuleFor(x => x.TelephoneNumber)
            .Must(phone => phone is null || (phone is not null && phone.All(c => char.IsDigit(c) || c == '+' || c == '-')))
            .WithMessage("Phone number contains invalid characters.");
    }
}
