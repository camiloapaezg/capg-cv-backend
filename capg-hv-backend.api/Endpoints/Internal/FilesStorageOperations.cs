using capg_hv_backend.Application.FilesValidator.Abstractions;
using capg_hv_backend.Application.FilesValidator.Entities;
using capg_hv_backend.Application.FilesValidator.Internal;
using capg_hv_backend.Application.Repositories.Abstractions;
using capg_hv_backend.Application.Repositories.Entities;
using capg_hv_backend.Domain.Entities;
using capg_hv_backend.Endpoints.Abstractions;
using capg_hv_backend.Endpoints.Entities;
using capg_hv_backend.Infrastructure.MessageBroker;
using capg_hv_backend.Infrastructure.MessageBroker.Abstractions;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text.Json;

namespace capg_hv_backend.Endpoints.Internal;

public class FilesStorageOperations(IOptions<MessageBrokerOptions> messageBrokerOptions,
                                    IListableRepository<FileMetaData> metadataRepository,
                                    IFilesRepository filesRepository,
                                    IFilesValidator filesValidator,
                                    IMessageBrokerClient messageBroker) : IFilesStorageOperations
{
    private readonly IFilesRepository _filesRepository = filesRepository ?? throw new ArgumentNullException(nameof(filesRepository));

    private readonly IFilesValidator _filesValidator = filesValidator ?? throw new ArgumentNullException(nameof(filesValidator));

    private readonly IMessageBrokerClient _messageBroker = messageBroker ?? throw new ArgumentNullException(nameof(messageBroker));

    private readonly MessageBrokerOptions _messageBrokerOptions = messageBrokerOptions?.Value ?? throw new ArgumentNullException(nameof(messageBrokerOptions));

    private readonly IListableRepository<FileMetaData> _metadataRepository = metadataRepository ?? throw new ArgumentNullException(nameof(metadataRepository));

    public async Task<FileHttpOperationResultDto<FileDownloadResultDto>> DownloadFileAsync(Guid fileId, CancellationToken token = default)
    {
        // Gets the metadata registry
        FileMetaData? metadata = await _metadataRepository.ReadAsync(fileId, token);
        if (metadata is null)
        {
            return new FileHttpOperationResultDto<FileDownloadResultDto>(HttpStatusCode.BadRequest, "The file is not registered in the database.");
        }

        // Gets the file
        FileOperationResult<byte[]> downloaded = await _filesRepository.DownloadAsync(metadata.Id, token);
        if (downloaded.StatusCode != HttpStatusCode.OK || downloaded.Data is null)
        {
            return new FileHttpOperationResultDto<FileDownloadResultDto>(HttpStatusCode.InternalServerError, downloaded.Message);
        }

        return new FileHttpOperationResultDto<FileDownloadResultDto>(HttpStatusCode.OK, Value: new FileDownloadResultDto(metadata.Name, downloaded.Data));
    }

    public async Task<FileHttpOperationResultDto<FileScanRequestDto>> StartUploadFileAsync(Guid userId, IFormFile file, CancellationToken token = default)
    {
        // Prepares file name
        string fileName = file.FileName;

        // Prepares file content.
        using MemoryStream stream = new();
        await file.CopyToAsync(stream, token);
        stream.Position = 0;

        // Validates file content
        FileValidationResult validationResult = await _filesValidator.ValidateFileAsync(stream.ToArray(), fileName, token);
        if (!validationResult.IsValid)
        {
            return new FileHttpOperationResultDto<FileScanRequestDto>(HttpStatusCode.BadRequest, validationResult.Message);
        }

        // Creates the request.
        fileName = FilesValidator.GetSanitizedFileName(fileName);
        FileScanRequestDto scanRequest = new(Guid.NewGuid(), userId, fileName);

        // Saves file to quarantine blob.
        FileOperationResult<string> uploaded = await _filesRepository.UploadToQuarantineAsync(scanRequest.Id, stream, token);
        if (uploaded.StatusCode != HttpStatusCode.OK)
        {
            return new FileHttpOperationResultDto<FileScanRequestDto>(HttpStatusCode.InternalServerError, uploaded.Message);
        }

        // Sends request to message broker queue.
        string queueName = _messageBrokerOptions.Queues.FilesScanner;
        byte[] body = JsonSerializer.SerializeToUtf8Bytes(scanRequest);
        await _messageBroker.SendMessageAsync(queueName, body, token);

        return new FileHttpOperationResultDto<FileScanRequestDto>(HttpStatusCode.Accepted, Value: scanRequest);
    }
}