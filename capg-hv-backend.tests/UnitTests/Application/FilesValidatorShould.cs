using capg_hv_backend.Application.FilesValidator.Abstractions;
using capg_hv_backend.Application.FilesValidator.Entities;
using capg_hv_backend.tests.Fixtures;
using capg_hv_backend.tests.Resources;
using Microsoft.Extensions.DependencyInjection;
using System.Collections;

namespace capg_hv_backend.tests.UnitTests.Application;

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

[Collection("Application services")]
public class FilesValidatorShould(ApplicationFixture fixture)
{
    private readonly ApplicationFixture _fixture = fixture;

    [Theory]
    [ClassData(typeof(FilesValidatorData))]
    public async Task ValidateFiles(byte[] content, string fileName, bool isValid)
    {
        IFilesValidator sut = _fixture.TestHost.Services.GetRequiredService<IFilesValidator>();
        Assert.NotNull(sut);

        ValidationResult result = await sut.ValidateFileAsync(content, fileName);
        Assert.Equal(isValid, result.IsValid);
    }
}