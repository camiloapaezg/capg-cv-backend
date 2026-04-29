using capg_hv_backend.Application.Entities;
using capg_hv_backend.Application.Helpers.Abstractions;
using Microsoft.Extensions.Options;
using System.Buffers.Binary;
using System.Net.Sockets;
using System.Text;

namespace capg_hv_backend.Application.Helpers.Internal;

public sealed class AntivirusServiceHelper(IOptions<AntivirusServiceOptions> options) : IAntivirusServiceHelper
{
    private readonly int _chunkSize = 64 * 1024;

    private readonly AntivirusServiceOptions _options = options?.Value ?? throw new ArgumentNullException(nameof(options));

    public async Task<AntivirusScanResult> ScanFileAsync(byte[] content, CancellationToken token = default)
    {
        return await ScanFileAsync(_options.Hostname, _options.Port, content, token);
    }

    public async Task<AntivirusScanResult> ScanFileAsync(string hostname, int port, byte[] content, CancellationToken token = default)
    {
        try
        {
            using TcpClient client = new(hostname, port);
            using NetworkStream stream = client.GetStream();

            // Starts stream
            await stream.WriteAsync(Encoding.UTF8.GetBytes("zINSTREAM\0"), token);

            // Writes content
            await WriteInChunksAsync(stream, content, token);

            // End of stream
            await stream.WriteAsync(new byte[] { 0, 0, 0, 0 }, token);

            // Receives response
            byte[] responseBuffer = new byte[1024];
            int bytesRead = await stream.ReadAsync(responseBuffer, token);
            string response = Encoding.UTF8.GetString(responseBuffer, 0, bytesRead);

            return GetResult(response);
        }
        catch (Exception ex)
        {
            return new AntivirusScanResult(AntivirusScanStatus.ScanError, ex.Message);
        }
    }

    public async Task WriteInChunksAsync(NetworkStream stream, byte[] content, CancellationToken token = default)
    {
        int position = 0;
        byte[] lengthBuffer = new byte[4];

        while (position < content.Length)
        {
            int remaining = content.Length - position;
            int currentSize = Math.Min(_chunkSize, remaining);

            // Convert length to Big Endian (Network Byte Order)
            BinaryPrimitives.WriteUInt32BigEndian(lengthBuffer, (uint)currentSize);

            // Write [Length Prefix] + [Actual Data]
            await stream.WriteAsync(lengthBuffer, token);
            await stream.WriteAsync(content.AsMemory(position, currentSize), token);

            position += currentSize;
        }
    }

    private static AntivirusScanResult GetResult(string input)
    {
        // Removes unnecessary information
        string response = input.Replace("stream:", "", StringComparison.InvariantCultureIgnoreCase);
        response = new string([.. response.Where(c => !char.IsControl(c))]);

        if (response.Contains("OK", StringComparison.InvariantCultureIgnoreCase))
        {
            return new AntivirusScanResult(AntivirusScanStatus.Clean);
        }

        if (response.Contains("FOUND", StringComparison.InvariantCultureIgnoreCase))
        {
            return new AntivirusScanResult(AntivirusScanStatus.Rejected, response.Replace(" FOUND", "", StringComparison.InvariantCultureIgnoreCase));
        }

        return new AntivirusScanResult(AntivirusScanStatus.Unknown);
    }
}