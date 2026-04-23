using capg_hv_backend.Application.Persistence.Internal;
using capg_hv_backend.Application.Repositories.Abstractions;
using capg_hv_backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace capg_hv_backend.Application.Repositories.Internal;

public sealed class WorkExperienceRepository(ApplicationDbContext context) : IRepository<WorkExperience>
{
    private readonly ApplicationDbContext _context = context;

    public async Task<WorkExperience?> Create(WorkExperience entity, CancellationToken token = default)
    {
        var experience = (WorkExperience)entity.Clone();
        if (experience.From.Kind != DateTimeKind.Utc)
        {
            experience.From = experience.From.ToUniversalTime();
        }

        if (experience.Until is not null && experience.Until.Value.Kind != DateTimeKind.Utc)
        {
            experience.Until = experience.Until.Value.ToUniversalTime();
        }

        var result = await _context.WorkExperience.AddAsync(experience, token);
        await _context.SaveChangesAsync(token);

        return result?.Entity;
    }

    public async Task<WorkExperience?> Delete(Guid id, CancellationToken token = default)
    {
        var existing = await Get(id, token);
        if (existing is null)
        {
            return null;
        }

        _context.WorkExperience.Entry(existing).State = EntityState.Deleted;
        await _context.SaveChangesAsync(token);

        return existing;
    }

    public async Task<WorkExperience?> Get(Guid id, CancellationToken token = default) => await _context.WorkExperience.FindAsync([id], token);

    public async Task<List<WorkExperience>> List(Guid? userId = null) => await _context.WorkExperience.Where(i => i.UserId == userId).ToListAsync();

    public async Task<WorkExperience?> Update(WorkExperience entity, CancellationToken token = default)
    {
        var existing = await Get(entity.Id, token);
        if (existing is null)
        {
            return null;
        }

        var experience = (WorkExperience)entity.Clone();
        if (experience.From.Kind != DateTimeKind.Utc)
        {
            experience.From = experience.From.ToUniversalTime();
        }

        if (experience.Until is not null && experience.Until.Value.Kind != DateTimeKind.Utc)
        {
            experience.Until = experience.Until.Value.ToUniversalTime();
        }

        _context.WorkExperience.Entry(existing).CurrentValues.SetValues(experience);
        await _context.SaveChangesAsync(token);

        return entity;
    }
}