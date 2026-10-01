using capg_hv_backend.Application.FilesValidator.Abstractions;
using capg_hv_backend.Application.FilesValidator.Entities;
using capg_hv_backend.tests.Fixtures;
using capg_hv_backend.tests.Resources;
using Microsoft.Extensions.DependencyInjection;
using System.Collections;

namespace capg_hv_backend.tests.UnitTests.Application;

internal class FilesValidatorData : IEnumerable<TheoryDataRow<byte[], string, bool>>
{
    private readonly List<TheoryDataRow<byte[], string, bool>> _data =
  [
      new TheoryDataRow<byte[], string, bool>(TestFiles.EXE, "file.exe", false),
        new TheoryDataRow<byte[], string, bool>(TestFiles.PNG, "file.png", true),
        new TheoryDataRow<byte[], string, bool>(TestFiles.PDF, "file.pdf", true),
        new TheoryDataRow<byte[], string, bool>(TestFiles.Bash, "file.sh", false),
        new TheoryDataRow<byte[], string, bool>(TestFiles.JPG, "file.jpg", true),
        new TheoryDataRow<byte[], string, bool>(TestFiles.JPG, string.Empty, false),
    ];

    public IEnumerator<TheoryDataRow<byte[], string, bool>> GetEnumerator() => _data.GetEnumerator();

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

        ValidationResult result = await sut.ValidateFileAsync(content, fileName, TestContext.Current.CancellationToken);
        Assert.Equal(isValid, result.IsValid);
    }
}