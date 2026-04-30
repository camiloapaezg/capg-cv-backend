using capg_hv_backend.Application.Repositories.Abstractions;
using capg_hv_backend.Application.Repositories.Internal;
using capg_hv_backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace capg_hv_backend.Application.Repositories;

public static class DependencyInjection
{
    public static IServiceCollection AddRepositories(this IServiceCollection services, IConfiguration configuration)
    {
        // EF configuration
        services.Configure<DatabaseOptions>(configuration.GetSection(nameof(DatabaseOptions)));
        DatabaseOptions? options = configuration.GetRequiredSection(nameof(DatabaseOptions)).Get<DatabaseOptions>();
        if (options is not null)
        {
            services.AddDbContext<ApplicationDbContext>(o => o.UseNpgsql(options.ConnectionString));
        }

        // Entities repositories
        services.AddTransient<IRepository<User>, UsersRepository>();
        services.AddTransient<IRepository<PersonalDetails>, PersonalDetailsRepository>();
        services.AddTransient<IRepository<GeneralDetails>, GeneralDetailsRepository>();
        services.AddTransient<IRepository<CertificationTraining>, CertificationTrainingRepository>();
        services.AddTransient<IRepository<FormalEducation>, FormalEducationRepository>();
        services.AddTransient<IRepository<Publication>, PublicationRepository>();
        services.AddTransient<IRepository<WorkExperience>, WorkExperienceRepository>();
        services.AddTransient<IRepository<FileMetaData>, FileMetaDataRepository>();

        // File storage
        services.Configure<FileStorageOptions>(configuration.GetSection(nameof(FileStorageOptions)));
        services.AddTransient<IFilesRepository, FilesRepository>();

        return services;
    }

    public static IHost UseRepositories(this IHost host)
    {
        using IServiceScope scope = host.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.Migrate();

        return host;
    }
}
