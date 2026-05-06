using capg_hv_backend.Infrastructure.FilesScanner.Entities;

namespace capg_hv_backend.Infrastructure.FilesScanner.Abstractions;

public interface IFilesScanner
{
    Task<FileScanResult> ScanAsync(byte[] content, CancellationToken token = default);

    Task<FileScanResult> ScanAsync(string hostname, int port, byte[] content, CancellationToken token = default);
}
