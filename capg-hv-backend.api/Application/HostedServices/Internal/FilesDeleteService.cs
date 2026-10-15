using capg_hv_backend.Application.Channels.Abstractions;
using capg_hv_backend.Application.Channels.Entities;
using capg_hv_backend.Application.Repositories.Abstractions;
using capg_hv_backend.Application.Repositories.Entities;
using System.Net;

namespace capg_hv_backend.Application.HostedServices.Internal;

public sealed class FilesDeleteService(ILogger<FilesDeleteService> logger, IServiceProvider serviceProvider) : BackgroundService
{
    private readonly ILogger<FilesDeleteService> _logger = logger;

    private readonly IServiceProvider _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            using IServiceScope scope = _serviceProvider.CreateScope();
            IChannel<FileDeleteRequestDto> channel = scope.ServiceProvider.GetRequiredService<IChannel<FileDeleteRequestDto>>();
            IFilesRepository filesRepository = scope.ServiceProvider.GetRequiredService<IFilesRepository>();

            // Receives messages from channel
            await foreach (FileDeleteRequestDto request in await channel.ReadAllAsync(stoppingToken))
            {
                // Deletes file.
                _logger.LogInformation("Deleting file with Id '{FileId}' and name '{FileName}' belonging to the user with Id '{UserId}'...", request.FileId, request.FileName, request.UserId);
                FileOperationResult<object> fileResult = await filesRepository.DeleteAsync(request.FileId, stoppingToken);
                if (fileResult.StatusCode != HttpStatusCode.NoContent)
                {
                    _logger.LogError("Exception thrown :{Message}", fileResult.Message);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Exception '{Source}' thrown: {Message} - {StackTrace}", ex.Source, ex.Message, ex.StackTrace);
        }
    }
}