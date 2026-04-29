using capg_hv_backend.Application.Validators.Internal;
using FluentValidation;

namespace capg_hv_backend.Application.Validators;

public static class DependencyInjection
{
    public static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<UserAddValidator>();
        services.AddValidatorsFromAssemblyContaining<UserUpdateValidator>();
        services.AddValidatorsFromAssemblyContaining<CertificationTrainingAddValidator>();
        services.AddValidatorsFromAssemblyContaining<CertificationTrainingUpdateValidator>();
        services.AddValidatorsFromAssemblyContaining<FormalEducationAddValidator>();
        services.AddValidatorsFromAssemblyContaining<FormalEducationUpdateValidator>();
        services.AddValidatorsFromAssemblyContaining<GeneralDetailsAddValidator>();
        services.AddValidatorsFromAssemblyContaining<GeneralDetailsUpdateValidator>();
        services.AddValidatorsFromAssemblyContaining<PersonalDetailsAddValidator>();
        services.AddValidatorsFromAssemblyContaining<PersonalDetailsUpdateValidator>();
        services.AddValidatorsFromAssemblyContaining<PublicationAddValidator>();
        services.AddValidatorsFromAssemblyContaining<PublicationUpdateValidator>();
        services.AddValidatorsFromAssemblyContaining<WorkExperienceAddValidator>();
        services.AddValidatorsFromAssemblyContaining<WorkExperienceUpdateValidator>();
        services.AddValidatorsFromAssemblyContaining<FileMetaDataAddValidator>();
        services.AddValidatorsFromAssemblyContaining<FileMetaDataUpdateValidator>();

        return services;
    }
}