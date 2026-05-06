using Polly;
using Polly.Registry;

namespace capg_hv_backend.Application.Middlewares.Internal;

public class ResilienceMiddleware(RequestDelegate next, ResiliencePipelineProvider<string> pipelineProvider)
{
    public static readonly string PipelineName = "resilient-pipeline";

    private readonly RequestDelegate _next = next;

    private readonly ResiliencePipelineProvider<string> _pipelineProvider = pipelineProvider;

    public async Task InvokeAsync(HttpContext httpContext)
    {
        ResiliencePipeline pipeline = _pipelineProvider.GetPipeline(PipelineName);

        await pipeline.ExecuteAsync(
            async (ct) => await _next(httpContext),
            httpContext.RequestAborted);
    }
}
