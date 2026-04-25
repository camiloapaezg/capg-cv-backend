using capg_hv_backend.Application.Persistence.Internal;
using capg_hv_backend.Application.Repositories.Abstractions;
using capg_hv_backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace capg_hv_backend.Application.Repositories.Internal;

public sealed class GeneralDetailsRepository(ApplicationDbContext context) : IRepository<GeneralDetails>
{
    private readonly ApplicationDbContext _context = context;

    public async Task<GeneralDetails?> Create(GeneralDetails entity, CancellationToken token = default)
    {
        EntityEntry<GeneralDetails> result = await _context.GeneralDetails.AddAsync(entity, token);
        await _context.SaveChangesAsync(token);

        return result?.Entity;
    }

    public async Task<GeneralDetails?> Delete(Guid id, CancellationToken token = default)
    {
        GeneralDetails? existing = await Get(id, token);
        if (existing is null)
        {
            return null;
        }

        _context.GeneralDetails.Entry(existing).State = EntityState.Deleted;
        await _context.SaveChangesAsync(token);

        return existing;
    }

    public async Task<bool> Exists(Guid id, CancellationToken token = default) => await _context.GeneralDetails.AnyAsync(e => e.Id == id, token);

    public async Task<GeneralDetails?> Get(Guid id, CancellationToken token = default) => await _context.GeneralDetails.FindAsync([id], token);

    public async Task<List<GeneralDetails>> List(Guid? userId = null) => await _context.GeneralDetails.Where(i => i.UserId == userId).ToListAsync();

    public async Task<GeneralDetails?> Update(GeneralDetails entity, CancellationToken token = default)
    {
        GeneralDetails? existing = await Get(entity.Id, token);
        if (existing is null)
        {
            return null;
        }

        _context.GeneralDetails.Entry(existing).CurrentValues.SetValues(entity);
        await _context.SaveChangesAsync(token);

        return entity;
    }
}