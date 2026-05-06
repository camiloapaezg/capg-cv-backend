namespace capg_hv_backend.Application.FilesValidator.Internal;

public static class FileSignaturesHelper
{
    private static byte[] Executable => [0x4D, 0x5A];
    private static byte[] Jpeg => [0xFF, 0xD8, 0xFF, 0xE0];
    private static byte[] JpegExif => [0xFF, 0xD8, 0xFF, 0xE1];
    private static byte[] Jpg => [0xFF, 0xD8, 0xFF, 0xDB];
    private static byte[] Jpg2 => [0xFF, 0xD8, 0xFF, 0xEE];
    private static byte[] Pdf => [0x25, 0x50, 0x44, 0x46, 0x2D];
    private static byte[] Png => [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];
    private static byte[] Script => [0x23, 0x21];

    public static bool IsExecutable(byte[] content) => IsSignatureMatch(content, Executable);

    public static bool IsImage(byte[] content)
    {
        List<byte[]> signatures = [Png, Jpg, Jpg2, Jpeg, JpegExif];
        foreach (byte[] signature in signatures)
        {
            bool matches = IsSignatureMatch(content, signature);
            if (matches)
            {
                return true;
            }
        }

        return false;
    }

    public static bool IsPdf(byte[] content) => IsSignatureMatch(content, Pdf);

    public static bool IsScript(byte[] content) => IsSignatureMatch(content, Script);

    private static bool IsSignatureMatch(byte[] content, byte[] signature)
    {
        if (content.Length < signature.Length)
        {
            return false;
        }

        return content.Take(signature.Length).SequenceEqual(signature);
    }
}