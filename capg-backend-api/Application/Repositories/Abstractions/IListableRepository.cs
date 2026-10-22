using capg_hv_backend.Domain.Entities;

namespace capg_hv_backend.Application.Repositories.Abstractions;

public interface IListableRepository<T> : ICrudRepository<T> where T : BaseEntity
{
    Task<List<T>> List(Guid? id = null, CancellationToken token = default);
}
