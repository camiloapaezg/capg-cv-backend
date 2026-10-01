using capg_hv_backend.Domain.Entities;

namespace capg_hv_backend.Application.Repositories.Abstractions;

public interface IRepository<T> where T : BaseEntity
{
    Task<T?> Create(T entity, CancellationToken token = default);

    Task<T?> Delete(Guid id, CancellationToken token = default);

    Task<bool> Exists(Guid id, CancellationToken token = default);

    Task<T?> Get(Guid id, CancellationToken token = default);

    Task<List<T>> List(Guid? id = null, CancellationToken token = default);

    Task<T?> Update(T entity, CancellationToken token = default);
}