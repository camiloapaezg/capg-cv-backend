namespace capg_hv_backend.Infrastructure.FilesScanner;

public sealed class FilesScannerOptions
{
    public string Hostname { get; set; } = null!;

    public int Port { get; set; }
}
