using capg_hv_backend.Application.Repositories.Abstractions;
using capg_hv_backend.Domain.Entities;
using capg_hv_backend.Infrastructure.Database.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace capg_hv_backend.Application.Repositories.Internal;

public sealed class CertificationTrainingRepository(ApplicationDbContext context) : IListableRepository<CertificationTraining>
{
    private readonly ApplicationDbContext _context = context;

    public async Task<CertificationTraining?> CreateAsync(CertificationTraining entity, CancellationToken token = default)
    {
        EntityEntry<CertificationTraining> result = await _context.CertificationTraining.AddAsync(entity, token);
        await _context.SaveChangesAsync(token);

        return result?.Entity;
    }

    public async Task<CertificationTraining?> DeleteAsync(Guid id, CancellationToken token = default)
    {
        CertificationTraining? existing = await ReadAsync(id, token);
        if (existing is null)
        {
            return null;
        }

        _context.CertificationTraining.Entry(existing).State = EntityState.Deleted;
        await _context.SaveChangesAsync(token);

        return existing;
    }

    public async Task<List<CertificationTraining>> List(Guid? userId = null, CancellationToken token = default)
    {
        User? user = await _context.Users.FindAsync([userId], token);
        if (user is null)
        {
            return [];
        }

        await _context.Entry(user).Collection(u => u.CertificationTraining).LoadAsync(token);
        return [.. user.CertificationTraining];
    }

    public async Task<CertificationTraining?> ReadAsync(Guid id, CancellationToken token = default) => await _context.CertificationTraining.FindAsync([id], token);

    public async Task<CertificationTraining?> UpdateAsync(CertificationTraining entity, CancellationToken token = default)
    {
        CertificationTraining? existing = await ReadAsync(entity.Id, token);
        if (existing is null)
        {
            return null;
        }

        _context.CertificationTraining.Entry(existing).CurrentValues.SetValues(entity);
        await _context.SaveChangesAsync(token);

        return entity;
    }
}