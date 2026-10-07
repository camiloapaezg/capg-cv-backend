using capg_hv_backend.Application.Channels.Abstractions;
using capg_hv_backend.Application.Channels.Entities;
using capg_hv_backend.Application.Middlewares.Internal;
using capg_hv_backend.Application.Repositories.Abstractions;
using capg_hv_backend.Application.Repositories.Entities;
using capg_hv_backend.Domain.Entities;
using Polly;
using System.Net;

namespace capg_hv_backend.Application.HostedServices.Internal;

public sealed class FilesDeleteService(ILogger<FilesDeleteService> logger, IServiceProvider serviceProvider) : BackgroundService
{
    private readonly ILogger<FilesDeleteService> _logger = logger;

    private readonly IServiceProvider _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using IServiceScope scope = _serviceProvider.CreateScope();
        ResiliencePipeline pipeline = scope.ServiceProvider.GetRequiredKeyedService<ResiliencePipeline>(ResilienceMiddleware.PipelineName); ;
        IChannel<FileDeleteRequestDto> channel = scope.ServiceProvider.GetRequiredService<IChannel<FileDeleteRequestDto>>();
        IFilesRepository filesRepository = scope.ServiceProvider.GetRequiredService<IFilesRepository>();
        IRepository<FileMetaData> metadataRepository = scope.ServiceProvider.GetRequiredService<IRepository<FileMetaData>>();

        // Receives messages from channel
        await foreach (FileDeleteRequestDto request in await channel.ReadAllAsync(stoppingToken))
        {
            // Executes with resiliency.
            await pipeline.ExecuteAsync(async token =>
            {
                // Deletes file.
                _logger.LogInformation("Deleting file with Id '{FileId}' and name '{FileName}'...", request.FileId, request.FileName);
                FileOperationResult<object> fileResult = await filesRepository.Delete(request.FileId, token).ConfigureAwait(false);
                if (fileResult.StatusCode != HttpStatusCode.NoContent)
                {
                    _logger.LogError("Exception thrown :{Message}", fileResult.Message);
                    throw new HttpRequestException(HttpRequestError.Unknown, statusCode: fileResult.StatusCode);
                }

                // Deletes metadata
                await metadataRepository.Delete(request.FileId, token).ConfigureAwait(false);
            }, stoppingToken);
        }
    }
}