using capg_hv_backend.Application.Repositories.Abstractions;
using capg_hv_backend.Domain.Entities;
using capg_hv_backend.Infrastructure.Database.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace capg_hv_backend.Application.Repositories.Internal;

public sealed class FormalEducationRepository(ApplicationDbContext context) : IListableRepository<FormalEducation>
{
    private readonly ApplicationDbContext _context = context;

    public async Task<FormalEducation?> CreateAsync(FormalEducation entity, CancellationToken token = default)
    {
        EntityEntry<FormalEducation> result = await _context.FormalEducation.AddAsync(entity, token);
        await _context.SaveChangesAsync(token);

        return result?.Entity;
    }

    public async Task<FormalEducation?> DeleteAsync(Guid id, CancellationToken token = default)
    {
        FormalEducation? existing = await ReadAsync(id, token);
        if (existing is null)
        {
            return null;
        }

        _context.FormalEducation.Entry(existing).State = EntityState.Deleted;
        await _context.SaveChangesAsync(token);

        return existing;
    }

    public async Task<List<FormalEducation>> List(Guid? userId = null, CancellationToken token = default)
    {
        User? user = await _context.Users.FindAsync([userId], token);
        if (user is null)
        {
            return [];
        }

        await _context.Entry(user).Collection(u => u.Education).LoadAsync(token);
        return [.. user.Education];
    }

    public async Task<FormalEducation?> ReadAsync(Guid id, CancellationToken token = default) => await _context.FormalEducation.FindAsync([id], token);

    public async Task<FormalEducation?> UpdateAsync(FormalEducation entity, CancellationToken token = default)
    {
        FormalEducation? existing = await ReadAsync(entity.Id, token);
        if (existing is null)
        {
            return null;
        }

        _context.FormalEducation.Entry(existing).CurrentValues.SetValues(entity);
        await _context.SaveChangesAsync(token);

        return entity;
    }
}