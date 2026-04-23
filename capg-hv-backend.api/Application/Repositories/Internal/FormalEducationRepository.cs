using capg_hv_backend.Application.Persistence.Internal;
using capg_hv_backend.Application.Repositories.Abstractions;
using capg_hv_backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace capg_hv_backend.Application.Repositories.Internal;

public sealed class FormalEducationRepository(ApplicationDbContext context) : IRepository<FormalEducation>
{
    private readonly ApplicationDbContext _context = context;

    public async Task<FormalEducation?> Create(FormalEducation entity, CancellationToken token = default)
    {
        var result = await _context.FormalEducation.AddAsync(entity, token);
        await _context.SaveChangesAsync(token);

        return result?.Entity;
    }

    public async Task<FormalEducation?> Delete(Guid id, CancellationToken token = default)
    {
        var existing = await Get(id, token);
        if (existing is null)
        {
            return null;
        }

        _context.FormalEducation.Entry(existing).State = EntityState.Deleted;
        await _context.SaveChangesAsync(token);

        return existing;
    }

    public async Task<FormalEducation?> Get(Guid id, CancellationToken token = default) => await _context.FormalEducation.FindAsync([id], token);

    public async Task<List<FormalEducation>> List(Guid? userId = null) => await _context.FormalEducation.Where(i => i.UserId == userId).ToListAsync();

    public async Task<FormalEducation?> Update(FormalEducation entity, CancellationToken token = default)
    {
        var existing = await Get(entity.Id, token);
        if (existing is null)
        {
            return null;
        }

        _context.FormalEducation.Entry(existing).CurrentValues.SetValues(entity);
        await _context.SaveChangesAsync(token);

        return entity;
    }
}