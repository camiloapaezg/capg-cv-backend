using capg_hv_backend.Application.Middlewares.Internal;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;

namespace capg_hv_backend.Application.Middlewares;

public static class DependencyInjection
{
    public static IServiceCollection AddCustomMiddleware(this IServiceCollection services)
    {
        services.AddResiliencePipeline("resilient-pipeline", (builder, context) =>
        {
            builder.AddTimeout(TimeSpan.FromSeconds(60))
            .AddRetry(new RetryStrategyOptions
            {
                MaxRetryAttempts = 3,
                BackoffType = DelayBackoffType.Exponential,
            })
            .AddCircuitBreaker(new CircuitBreakerStrategyOptions
            {
                FailureRatio = 0.5,
                SamplingDuration = TimeSpan.FromSeconds(30),
                MinimumThroughput = 10,
                BreakDuration = TimeSpan.FromSeconds(30)
            });
        });

        return services;
    }

    public static IApplicationBuilder UseCustomMiddleware(this IApplicationBuilder builder)
    {
        builder.UseMiddleware<ExceptionHandlingMiddleware>();
        builder.UseMiddleware<SecurityHeadersMiddleware>();
        builder.UseMiddleware<ResilienceMiddleware>();

        return builder;
    }
}