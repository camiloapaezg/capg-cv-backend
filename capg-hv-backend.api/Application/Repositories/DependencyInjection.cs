using capg_hv_backend.Application.Repositories.Abstractions;
using capg_hv_backend.Application.Repositories.Internal;
using capg_hv_backend.Domain.Entities;

namespace capg_hv_backend.Application.Repositories;

public static class DependencyInjection
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddTransient<IRepository<User>, UsersRepository>();
        services.AddTransient<IRepository<PersonalDetails>, PersonalDetailsRepository>();
        return services;
    }
}
