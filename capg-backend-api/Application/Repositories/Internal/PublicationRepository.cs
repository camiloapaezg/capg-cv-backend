using capg_hv_backend.Application.Repositories.Abstractions;
using capg_hv_backend.Domain.Entities;
using capg_hv_backend.Infrastructure.Database.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace capg_hv_backend.Application.Repositories.Internal;

public sealed class PublicationRepository(ApplicationDbContext context) : IListableRepository<Publication>
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Publication?> CreateAsync(Publication entity, CancellationToken token = default)
    {
        EntityEntry<Publication> result = await _context.Publications.AddAsync(entity, token);
        await _context.SaveChangesAsync(token);

        return result?.Entity;
    }

    public async Task<Publication?> DeleteAsync(Guid id, CancellationToken token = default)
    {
        Publication? existing = await ReadAsync(id, token);
        if (existing is null)
        {
            return null;
        }

        _context.Publications.Entry(existing).State = EntityState.Deleted;
        await _context.SaveChangesAsync(token);

        return existing;
    }

    public async Task<List<Publication>> List(Guid? userId = null, CancellationToken token = default)
    {
        User? user = await _context.Users.FindAsync([userId], token);
        if (user is null)
        {
            return [];
        }

        await _context.Entry(user).Collection(u => u.Publications).LoadAsync(token);
        return [.. user.Publications];
    }

    public async Task<Publication?> ReadAsync(Guid id, CancellationToken token = default) => await _context.Publications.FindAsync([id], token);

    public async Task<Publication?> UpdateAsync(Publication entity, CancellationToken token = default)
    {
        Publication? existing = await ReadAsync(entity.Id, token);
        if (existing is null)
        {
            return null;
        }

        _context.Publications.Entry(existing).CurrentValues.SetValues(entity);
        await _context.SaveChangesAsync(token);

        return entity;
    }
}