using capg_hv_backend.Application.FilesValidator.Abstractions;
using capg_hv_backend.Application.FilesValidator.Entities;
using Microsoft.Extensions.Options;
using NetVips;

namespace capg_hv_backend.Application.FilesValidator.Internal;

public class FilesValidator(IOptions<FileValidationOptions> options) : IFilesValidator
{
    private readonly FileValidationOptions _options = options?.Value ?? throw new ArgumentNullException(nameof(options));

    public static string GetSanitizedFileName(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            return string.Empty;
        }

        char[] invalidChars = Path.GetInvalidFileNameChars();
        return new string([.. fileName.Where(c => !invalidChars.Contains(c))]);
    }

    public static FileValidationResult IsFileTypeValid(byte[] content)
    {
        if (FileSignaturesHelper.IsImage(content) || FileSignaturesHelper.IsPdf(content))
        {
            return new FileValidationResult();
        }

        if (FileSignaturesHelper.IsExecutable(content) || FileSignaturesHelper.IsScript(content))
        {
            return new FileValidationResult(false, "The file type is invalid.");
        }

        return new FileValidationResult(false, "The file type is not supported.");
    }

    public async Task<FileValidationResult> ValidateFileAsync(byte[] content, string fileName, CancellationToken token = default)
    {
        // Checks size
        if (content.Length == 0 || content.Length > _options.MaxSizeInBytes)
        {
            return new FileValidationResult(false, $"The content size cannot be null nor bigger than {_options.MaxSizeInBytes / (1024 * 1024)} Mb");
        }

        // Checks file name
        string name = GetSanitizedFileName(fileName);
        if (string.IsNullOrEmpty(name))
        {
            return new FileValidationResult(false, "The file name is not valid");
        }

        // Checks file type
        FileValidationResult result = IsFileTypeValid(content);
        if (!result.IsValid)
        {
            return result;
        }

        // Checks image integrity
        if (FileSignaturesHelper.IsImage(content))
        {
            try
            {
                using MemoryStream stream = new(content);
                Image image = Image.NewFromStream(stream, access: Enums.Access.Sequential);
                ArgumentNullException.ThrowIfNull(image);
            }
            catch (TaskCanceledException)
            {
                return new FileValidationResult(false, "The task was cancelled");
            }
            catch (Exception ex)
            {
                return new FileValidationResult(false, "The image is corrupted: " + ex.Message);
            }
        }

        return new FileValidationResult();
    }
}