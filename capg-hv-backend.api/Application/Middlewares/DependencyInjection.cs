using capg_hv_backend.Application.Middlewares.Internal;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;

namespace capg_hv_backend.Application.Middlewares;

public static class DependencyInjection
{
    public static IServiceCollection AddCustomMiddleware(this IServiceCollection services)
    {
        services.AddResiliencePipeline(ResilienceMiddleware.PipelineName, (builder, context) =>
        {
            builder.AddRetry(new RetryStrategyOptions
            {
                MaxRetryAttempts = 3,
                BackoffType = DelayBackoffType.Exponential,
                ShouldHandle = (args) => args.Outcome switch
                {
                    { Exception: HttpRequestException } => PredicateResult.True(),
                    { Exception: TimeoutRejectedException } => PredicateResult.True(),
                    { Result: HttpResponseMessage response } when !response.IsSuccessStatusCode => PredicateResult.True(),
                    _ => PredicateResult.False(),
                }
            })
            .AddCircuitBreaker(new CircuitBreakerStrategyOptions
            {
                FailureRatio = 0.5,
                SamplingDuration = TimeSpan.FromSeconds(30),
                MinimumThroughput = 10,
                BreakDuration = TimeSpan.FromSeconds(30)
            })
            .AddTimeout(TimeSpan.FromSeconds(60));
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