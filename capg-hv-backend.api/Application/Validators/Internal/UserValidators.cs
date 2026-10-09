using capg_hv_backend.Domain.Entities;
using capg_hv_backend.Endpoints.Entities;
using FluentValidation;

namespace capg_hv_backend.Application.Validators.Internal;

public class UserAddValidator: AbstractValidator<UserCreateRequestDto>
{
    public UserAddValidator()
    {
        RuleFor(x => x.FirstName).NotNull().NotEmpty();
        RuleFor(x => x.LastName).NotNull().NotEmpty();
        RuleFor(x => x.EmailAddress).EmailAddress();
    }
}

public class UserUpdateValidator : AbstractValidator<User>
{
    public UserUpdateValidator()
    {
        RuleFor(x => x.FirstName).NotNull().NotEmpty();
        RuleFor(x => x.LastName).NotNull().NotEmpty();
        RuleFor(x => x.EmailAddress).EmailAddress();
        RuleFor(x => x.Id).NotNull().NotEmpty();
    }
}

