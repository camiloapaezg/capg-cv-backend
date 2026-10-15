using capg_hv_backend.Application.Repositories.Abstractions;
using capg_hv_backend.Domain.Entities;
using capg_hv_backend.Infrastructure.Database.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace capg_hv_backend.Application.Repositories.Internal;

public sealed class GeneralDetailsRepository(ApplicationDbContext context) : ICrudRepository<GeneralDetails>
{
    private readonly ApplicationDbContext _context = context;

    public async Task<GeneralDetails?> CreateAsync(GeneralDetails entity, CancellationToken token = default)
    {
        EntityEntry<GeneralDetails> result = await _context.GeneralDetails.AddAsync(entity, token);
        await _context.SaveChangesAsync(token);

        return result?.Entity;
    }

    public async Task<GeneralDetails?> DeleteAsync(Guid userId, CancellationToken token = default)
    {
        GeneralDetails? existing = await ReadAsync(userId, token);
        if (existing is null)
        {
            return null;
        }

        _context.GeneralDetails.Entry(existing).State = EntityState.Deleted;
        await _context.SaveChangesAsync(token);

        return existing;
    }

    public async Task<GeneralDetails?> ReadAsync(Guid userId, CancellationToken token = default)
    {
        // Gets the user
        User? user = await _context.Users.FindAsync([userId], token);
        if (user is null)
        {
            return null;
        }

        // Loads the general details
        await _context.Entry(user).Reference(u => u.GeneralDetails).LoadAsync(token);

        return user.GeneralDetails;
    }

    public async Task<GeneralDetails?> UpdateAsync(GeneralDetails entity, CancellationToken token = default)
    {
        GeneralDetails? existing = await ReadAsync(entity.UserId, token);
        if (existing is null)
        {
            return null;
        }

        _context.GeneralDetails.Entry(existing).CurrentValues.SetValues(entity);
        await _context.SaveChangesAsync(token);

        return entity;
    }
}