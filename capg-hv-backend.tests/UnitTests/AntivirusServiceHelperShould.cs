using capg_hv_backend.Application.Entities;
using capg_hv_backend.Application.Helpers.Abstractions;
using capg_hv_backend.tests.Fixtures;
using capg_hv_backend.tests.Resources;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

namespace capg_hv_backend.tests.UnitTests;

[Collection("Helpers collection")]
public class AntivirusServiceHelperShould(HelpersFixture fixture)
{
    private readonly HelpersFixture _fixture = fixture;

    [Fact]
    public async Task ScanPDF_ReturnClean()
    {
        IAntivirusServiceHelper sut = _fixture.TestHost.Services.GetRequiredService<IAntivirusServiceHelper>();
        Assert.NotNull(sut);

        AntivirusScanResult result = await sut.ScanFileAsync(TestFiles.PDF);
        Assert.Equal(AntivirusScanStatus.Clean, result.Status);
    }

    [Fact]
    public async Task ScanIECAR_ReturnRejected()
    {
        IAntivirusServiceHelper sut = _fixture.TestHost.Services.GetRequiredService<IAntivirusServiceHelper>();
        Assert.NotNull(sut);

        string iecar = @"X5O!P%@AP[4\PZX54(P^)7CC)7}$EICAR-STANDARD-ANTIVIRUS-TEST-FILE!$H+H*";
        byte[] content = Encoding.UTF8.GetBytes(iecar);
        string base64 = Convert.ToBase64String(content);
        content = Convert.FromBase64String(base64);

        AntivirusScanResult result = await sut.ScanFileAsync(content);
        Assert.Equal(AntivirusScanStatus.Rejected, result.Status);
    }
}
