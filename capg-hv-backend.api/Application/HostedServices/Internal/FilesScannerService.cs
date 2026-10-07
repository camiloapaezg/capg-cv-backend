using capg_hv_backend.Application.Middlewares.Internal;
using capg_hv_backend.Application.Repositories.Abstractions;
using capg_hv_backend.Application.Repositories.Entities;
using capg_hv_backend.Domain.Entities;
using capg_hv_backend.Endpoints.Entities;
using capg_hv_backend.Infrastructure.FilesScanner.Abstractions;
using capg_hv_backend.Infrastructure.FilesScanner.Entities;
using capg_hv_backend.Infrastructure.MessageBroker;
using capg_hv_backend.Infrastructure.MessageBroker.Abstractions;
using Microsoft.Extensions.Options;
using Polly;
using RabbitMQ.Client.Events;
using System.Net;
using System.Text.Json;

namespace capg_hv_backend.Application.HostedServices.Internal;

public class FilesScannerService(ILogger<FilesScannerService> logger, IOptions<MessageBrokerOptions> options, IServiceProvider serviceProvider) : IHostedService
{
    private readonly ILogger<FilesScannerService> _logger = logger;

    private readonly string _queueName = options?.Value.Queues.FilesScanner ?? throw new ArgumentNullException(nameof(options));

    private readonly IServiceProvider _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

    private IMessageBroker? _messageBroker = null;

    private IServiceScope? _scope = null;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _scope = _serviceProvider.CreateScope();
        _messageBroker = _scope.ServiceProvider.GetRequiredService<IMessageBroker>();
        await _messageBroker.RegisterMessageHandler(_queueName, OnMessageReceived, cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _messageBroker?.Dispose();
        _scope?.Dispose();
    }

    private async Task OnMessageReceived(object sender, BasicDeliverEventArgs args)
    {
        _logger.LogInformation("Message received from queue '{RoutingKey}'.", args.RoutingKey);

        byte[] body = args.Body.ToArray();
        FileScanRequestDto? request = JsonSerializer.Deserialize<FileScanRequestDto>(body);
        if (request is null)
        {
            return;
        }

        _logger.LogInformation("Scanning file with Id '{Id}' and name '{FileName}' belonging to the user '{UserId}'...", request.Id, request.FileName, request.UserId);
        using IServiceScope scope = _serviceProvider.CreateScope();
        ResiliencePipeline pipeline = scope.ServiceProvider.GetRequiredKeyedService<ResiliencePipeline>(ResilienceMiddleware.PipelineName);
        IFilesScanner fileScanner = scope.ServiceProvider.GetRequiredService<IFilesScanner>();
        IFilesRepository filesRepository = scope.ServiceProvider.GetRequiredService<IFilesRepository>();

        // Gets the file from quarantine
        FileOperationResult<byte[]> downloaded = await pipeline.ExecuteAsync(async (token) => await filesRepository.DownloadFromQuarantine(request.Id, token)).ConfigureAwait(false);
        if (downloaded.StatusCode != HttpStatusCode.OK || downloaded.Data is null)
        {
            return;
        }

        // Scans file content with antivirus service
        FileScanResult fileScanResult = await pipeline.ExecuteAsync(async (token) => await fileScanner.ScanAsync(downloaded.Data, token)).ConfigureAwait(false);
        switch (fileScanResult.Status)
        {
            case FileScanStatus.Clean:
                string md5Hash = string.Empty;
                using (MemoryStream stream = new(downloaded.Data))
                {
                    // Saves to permanent storage.
                    FileOperationResult<string> created = await pipeline.ExecuteAsync(async (token) => await filesRepository.Upload(request.Id, stream, token)).ConfigureAwait(false);
                    if (created.StatusCode != HttpStatusCode.OK || created.Data is null)
                    {
                        return;
                    }

                    md5Hash = created.Data;
                }

                // Saves metadata
                FileMetaData metadata = new()
                {
                    Id = request.Id,
                    OwnerId = request.UserId,
                    Name = request.FileName,
                    SizeInBytes = downloaded.Data.Length,
                    ModifiedAt = DateTime.UtcNow,
                    Md5Hash = md5Hash,
                };

                IRepository<FileMetaData> metadataRepository = scope.ServiceProvider.GetRequiredService<IRepository<FileMetaData>>();
                await pipeline.ExecuteAsync(async (token) => await metadataRepository.Create(metadata, token)).ConfigureAwait(false);

                break;

            case FileScanStatus.Rejected:
                _logger.LogWarning("File rejected: {Mesage}", fileScanResult.Message);
                break;

            case FileScanStatus.ScanError:
                _logger.LogError("File scan failed: {Mesage}", fileScanResult.Message);
                break;
        }

        // Deletes from quarantine
        await pipeline.ExecuteAsync(async (token) => await filesRepository.DeleteFromQuarantine(request.Id, token)).ConfigureAwait(false);
    }
}