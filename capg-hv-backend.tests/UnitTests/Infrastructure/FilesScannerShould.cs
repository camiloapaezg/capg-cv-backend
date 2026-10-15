using capg_hv_backend.Infrastructure.FilesScanner.Abstractions;
using capg_hv_backend.Infrastructure.FilesScanner.Entities;
using capg_hv_backend.tests.Fixtures;
using capg_hv_backend.tests.Resources;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

namespace capg_hv_backend.tests.UnitTests.Infrastructure;

[Collection("Infrastructure services")]
public class FilesScannerShould(InfrastructureFixture fixture)
{
    private readonly InfrastructureFixture _fixture = fixture;

    [Fact]
    public async Task ScanIECAR_ReturnRejected()
    {
        IFilesScannerClient sut = _fixture.TestHost.Services.GetRequiredService<IFilesScannerClient>();
        Assert.NotNull(sut);

        string iecar = @"X5O!P%@AP[4\PZX54(P^)7CC)7}$EICAR-STANDARD-ANTIVIRUS-TEST-FILE!$H+H*";
        byte[] content = Encoding.UTF8.GetBytes(iecar);
        string base64 = Convert.ToBase64String(content);
        content = Convert.FromBase64String(base64);

        FileScanResult result = await sut.ScanAsync(content, TestContext.Current.CancellationToken);
        Assert.Equal(FileScanStatus.Rejected, result.Status);
    }

    [Fact]
    public async Task ScanPDF_ReturnClean()
    {
        IFilesScannerClient sut = _fixture.TestHost.Services.GetRequiredService<IFilesScannerClient>();
        Assert.NotNull(sut);

        FileScanResult result = await sut.ScanAsync(TestFiles.PDF, TestContext.Current.CancellationToken);
        Assert.Equal(FileScanStatus.Clean, result.Status);
    }
}