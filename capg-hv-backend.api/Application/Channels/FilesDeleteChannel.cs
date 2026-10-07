using capg_hv_backend.Application.Channels.Abstractions;
using capg_hv_backend.Application.Channels.Entities;
using System.Threading.Channels;

namespace capg_hv_backend.Application.Channels;

public sealed class FilesDeleteChannel : IChannel<FileDeleteRequestDto>
{
    private readonly Channel<FileDeleteRequestDto> _channel = Channel.CreateUnbounded<FileDeleteRequestDto>();

    public async Task<IAsyncEnumerable<FileDeleteRequestDto>> ReadAllAsync(CancellationToken token)
    {
        return _channel.Reader.ReadAllAsync(cancellationToken: token);
    }

    public async Task WriteAsync(FileDeleteRequestDto message, CancellationToken token = default)
    {
        await _channel.Writer.WriteAsync(message, token);
    }
}