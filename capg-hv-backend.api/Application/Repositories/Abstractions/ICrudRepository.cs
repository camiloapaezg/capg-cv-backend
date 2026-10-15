using capg_hv_backend.Domain.Entities;

namespace capg_hv_backend.Application.Repositories.Abstractions;

public interface ICrudRepository<T> where T : BaseEntity
{
    Task<T?> CreateAsync(T entity, CancellationToken token = default);

    Task<T?> DeleteAsync(Guid id, CancellationToken token = default);

    Task<T?> ReadAsync(Guid id, CancellationToken token = default);

    Task<T?> UpdateAsync(T entity, CancellationToken token = default);
}