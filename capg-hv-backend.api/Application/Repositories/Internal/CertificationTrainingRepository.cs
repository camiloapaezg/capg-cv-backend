using capg_hv_backend.Application.Persistence.Internal;
using capg_hv_backend.Application.Repositories.Abstractions;
using capg_hv_backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace capg_hv_backend.Application.Repositories.Internal;

public sealed class CertificationTrainingRepository(ApplicationDbContext context) : IRepository<CertificationTraining>
{
    private readonly ApplicationDbContext _context = context;

    public async Task<CertificationTraining?> Create(CertificationTraining entity, CancellationToken token = default)
    {
        var result = await _context.CertificationTraining.AddAsync(entity, token);
        await _context.SaveChangesAsync(token);

        return result?.Entity;
    }

    public async Task<CertificationTraining?> Delete(Guid id, CancellationToken token = default)
    {
        var existing = await Get(id, token);
        if (existing is null)
        {
            return null;
        }

        _context.CertificationTraining.Entry(existing).State = EntityState.Deleted;
        await _context.SaveChangesAsync(token);

        return existing;
    }

    public async Task<CertificationTraining?> Get(Guid id, CancellationToken token = default) => await _context.CertificationTraining.FindAsync([id], token);

    public async Task<List<CertificationTraining>> List(Guid? userId = null) => await _context.CertificationTraining.Where(i => i.UserId == userId).ToListAsync();

    public async Task<CertificationTraining?> Update(CertificationTraining entity, CancellationToken token = default)
    {
        var existing = await Get(entity.Id, token);
        if (existing is null)
        {
            return null;
        }

        _context.CertificationTraining.Entry(existing).CurrentValues.SetValues(entity);
        await _context.SaveChangesAsync(token);

        return entity;
    }
}