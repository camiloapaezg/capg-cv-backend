using capg_hv_backend.Application.Repositories.Abstractions;
using capg_hv_backend.Application.Repositories.Internal;
using capg_hv_backend.Domain.Entities;

namespace capg_hv_backend.Application.Repositories;

public static class DependencyInjection
{
    public static IServiceCollection AddRepositories(this IServiceCollection services, IConfiguration configuration)
    {
        // Adds options
        services.Configure<RepositoriesOptions>(configuration.GetSection(nameof(RepositoriesOptions)));

        // Entities repositories
        services.AddTransient<IListableRepository<User>, UsersRepository>();
        services.AddTransient<ICrudRepository<PersonalDetails>, PersonalDetailsRepository>();
        services.AddTransient<ICrudRepository<GeneralDetails>, GeneralDetailsRepository>();
        services.AddTransient<IListableRepository<CertificationTraining>, CertificationTrainingRepository>();
        services.AddTransient<IListableRepository<FormalEducation>, FormalEducationRepository>();
        services.AddTransient<IListableRepository<Publication>, PublicationRepository>();
        services.AddTransient<IListableRepository<WorkExperience>, WorkExperienceRepository>();
        services.AddTransient<IListableRepository<FileMetaData>, FileMetaDataRepository>();

        // Files storage repository
        services.AddTransient<IFilesRepository, FilesRepository>();

        return services;
    }
}
