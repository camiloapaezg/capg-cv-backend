using capg_hv_backend.Application.Persistence.Internal;
using capg_hv_backend.Application.Repositories.Abstractions;
using capg_hv_backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace capg_hv_backend.Application.Repositories.Internal;

public sealed class PublicationRepository(ApplicationDbContext context) : IRepository<Publication>
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Publication?> Create(Publication entity, CancellationToken token = default)
    {
        var result = await _context.Publications.AddAsync(entity, token);
        await _context.SaveChangesAsync(token);

        return result?.Entity;
    }

    public async Task<Publication?> Delete(Guid id, CancellationToken token = default)
    {
        var existing = await Get(id, token);
        if (existing is null)
        {
            return null;
        }

        _context.Publications.Entry(existing).State = EntityState.Deleted;
        await _context.SaveChangesAsync(token);

        return existing;
    }

    public async Task<Publication?> Get(Guid id, CancellationToken token = default) => await _context.Publications.FindAsync([id], token);

    public async Task<List<Publication>> List(Guid? userId = null) => await _context.Publications.Where(i => i.UserId == userId).ToListAsync();

    public async Task<Publication?> Update(Publication entity, CancellationToken token = default)
    {
        var existing = await Get(entity.Id, token);
        if (existing is null)
        {
            return null;
        }

        _context.Publications.Entry(existing).CurrentValues.SetValues(entity);
        await _context.SaveChangesAsync(token);

        return entity;
    }
}