namespace capg_hv_backend.Application.Repositories;

public sealed class FileStorageOptions
{
    public string AccessKey { get; set; } = null!;

    public string Hostname { get; set; } = null!;

    public int Port { get; set; }

    public string SecretKey { get; set; } = null!;
}