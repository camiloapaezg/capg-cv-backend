using capg_hv_backend.Application.Repositories.Abstractions;
using capg_hv_backend.Domain.Entities;
using capg_hv_backend.Infrastructure.Database.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace capg_hv_backend.Application.Repositories.Internal;

public sealed class WorkExperienceRepository(ApplicationDbContext context) : IListableRepository<WorkExperience>
{
    private readonly ApplicationDbContext _context = context;

    public async Task<WorkExperience?> CreateAsync(WorkExperience entity, CancellationToken token = default)
    {
        WorkExperience experience = (WorkExperience)entity.Clone();
        if (experience.From.Kind != DateTimeKind.Utc)
        {
            experience.From = experience.From.ToUniversalTime();
        }

        if (experience.Until is not null && experience.Until.Value.Kind != DateTimeKind.Utc)
        {
            experience.Until = experience.Until.Value.ToUniversalTime();
        }

        EntityEntry<WorkExperience> result = await _context.WorkExperience.AddAsync(experience, token);
        await _context.SaveChangesAsync(token);

        return result?.Entity;
    }

    public async Task<WorkExperience?> DeleteAsync(Guid id, CancellationToken token = default)
    {
        WorkExperience? existing = await ReadAsync(id, token);
        if (existing is null)
        {
            return null;
        }

        _context.WorkExperience.Entry(existing).State = EntityState.Deleted;
        await _context.SaveChangesAsync(token);

        return existing;
    }

    public async Task<List<WorkExperience>> List(Guid? userId = null, CancellationToken token = default)
    {
        User? user = await _context.Users.FindAsync([userId], token);
        if (user is null)
        {
            return [];
        }

        await _context.Entry(user).Collection(u => u.WorkExperience).LoadAsync(token);
        return [.. user.WorkExperience];
    }

    public async Task<WorkExperience?> ReadAsync(Guid id, CancellationToken token = default) => await _context.WorkExperience.FindAsync([id], token);

    public async Task<WorkExperience?> UpdateAsync(WorkExperience entity, CancellationToken token = default)
    {
        WorkExperience? existing = await ReadAsync(entity.Id, token);
        if (existing is null)
        {
            return null;
        }

        WorkExperience experience = (WorkExperience)entity.Clone();
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