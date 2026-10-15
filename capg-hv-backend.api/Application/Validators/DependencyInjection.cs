using capg_hv_backend.Application.Validators.Internal;
using FluentValidation;

namespace capg_hv_backend.Application.Validators;

public static class DependencyInjection
{
    public static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<UserValidator>();
        services.AddValidatorsFromAssemblyContaining<CertificationTrainingValidator>();
        services.AddValidatorsFromAssemblyContaining<FormalEducationValidator>();
        services.AddValidatorsFromAssemblyContaining<GeneralDetailsValidator>();
        services.AddValidatorsFromAssemblyContaining<PersonalDetailsValidator>();
        services.AddValidatorsFromAssemblyContaining<PublicationValidator>();
        services.AddValidatorsFromAssemblyContaining<WorkExperienceValidator>();

        return services;
    }
}