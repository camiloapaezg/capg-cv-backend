using capg_hv_backend.Application.Entities;

namespace capg_hv_backend.Application.Helpers.Abstractions;

public interface IAntivirusServiceHelper
{
    Task<AntivirusScanResult> ScanFileAsync(byte[] content, CancellationToken token = default);

    Task<AntivirusScanResult> ScanFileAsync(string hostname, int port, byte[] content, CancellationToken token = default);
}
