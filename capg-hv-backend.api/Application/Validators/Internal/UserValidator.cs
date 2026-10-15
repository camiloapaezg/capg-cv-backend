using capg_hv_backend.Endpoints.Entities;
using FluentValidation;

namespace capg_hv_backend.Application.Validators.Internal;

public class UserValidator : AbstractValidator<UserDto>
{
    public UserValidator()
    {
        RuleFor(x => x.FirstName).NotNull().NotEmpty();
        RuleFor(x => x.LastName).NotNull().NotEmpty();
        RuleFor(x => x.EmailAddress).EmailAddress().NotNull().NotEmpty();
        RuleFor(x => x.BirthDate).InclusiveBetween(DateTime.MinValue, DateTime.UtcNow).NotNull().NotEmpty();
        RuleFor(x => x.TelephoneNumber)
            .Must(phone => phone is null || (phone is not null && phone.All(c => char.IsDigit(c) || c == '+' || c == '-')))
            .WithMessage("Phone number contains invalid characters.");
    }
}


