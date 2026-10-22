namespace capg_hv_backend.Application.Channels.Abstractions;

public interface IChannel<T> where T : class
{
    public Task WriteAsync(T message, CancellationToken token = default);

    public Task<IAsyncEnumerable<T>> ReadAllAsync(CancellationToken token = default);
}
