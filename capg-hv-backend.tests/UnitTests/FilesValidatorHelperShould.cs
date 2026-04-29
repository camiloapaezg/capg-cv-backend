using capg_hv_backend.Application.Entities;
using capg_hv_backend.Application.Helpers.Abstractions;
using capg_hv_backend.tests.Fixtures;
using capg_hv_backend.tests.Resources;
using Microsoft.Extensions.DependencyInjection;
using System.Collections;

namespace capg_hv_backend.tests.UnitTests;

internal class FilesValidatorData : IEnumerable<object[]>
{
    private readonly List<object[]> _data =
    [
        [TestFiles.EXE, "file.exe", false],
        [TestFiles.PNG, "file.png", true],
        [TestFiles.PDF, "file.pdf", true],
        [TestFiles.Bash, "file.sh", false],
        [TestFiles.JPG, "file.jpg", true],
        [TestFiles.JPG, string.Empty, false],
    ];

    public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

[Collection("Helpers collection")]
public class FilesValidatorHelperShould(HelpersFixture fixture)
{
    private readonly HelpersFixture _fixture = fixture;

    [Theory]
    [ClassData(typeof(FilesValidatorData))]
    public async Task ValidateFiles(byte[] content, string fileName, bool isValid)
    {
        IFilesValidatorHelper sut = _fixture.TestHost.Services.GetRequiredService<IFilesValidatorHelper>();
        Assert.NotNull(sut);

        ValidationResult result = await sut.ValidateFileAsync(content, fileName);
        Assert.Equal(isValid, result.IsValid);
    }
}