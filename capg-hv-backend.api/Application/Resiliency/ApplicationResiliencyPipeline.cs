using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;
using System.Net;
using System.Net.Sockets;

namespace capg_hv_backend.Application.Resiliency;

public static class ApplicationResiliencyPipeline
{
    public static readonly string PipelineName = "resiliency-pipeline";

    private static readonly int _maxRetryAttempts = 3;

    private static readonly TimeSpan _timeout = TimeSpan.FromSeconds(60);

    public static ResiliencePipelineBuilder GetResiliencyPipeline(ResiliencePipelineBuilder builder)
    {
        PredicateBuilder<object> shouldHandle = new PredicateBuilder()
                            .Handle<HttpRequestException>(ex => ex.StatusCode != HttpStatusCode.BadRequest)
                            .Handle<TimeoutRejectedException>()
                            .Handle<SocketException>();

        builder.AddRetry(new RetryStrategyOptions
        {
            MaxRetryAttempts = _maxRetryAttempts,
            BackoffType = DelayBackoffType.Exponential,
            ShouldHandle = shouldHandle
        }).AddCircuitBreaker(new CircuitBreakerStrategyOptions
        {
            FailureRatio = 0.5,
            SamplingDuration = TimeSpan.FromSeconds(30),
            MinimumThroughput = 10,
            BreakDuration = TimeSpan.FromSeconds(30),
            ShouldHandle = shouldHandle
        }).AddTimeout(_timeout);

        return builder;
    }
}