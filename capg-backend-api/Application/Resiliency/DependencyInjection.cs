using Polly;

namespace capg_hv_backend.Application.Resiliency;

public static class DependencyInjection
{
    public static IServiceCollection AddResiliency(this IServiceCollection services)
    {
        services.AddResiliencePipeline(ApplicationResiliencyPipeline.PipelineName, builder => ApplicationResiliencyPipeline.GetResiliencyPipeline(builder));

        return services;
    }
}