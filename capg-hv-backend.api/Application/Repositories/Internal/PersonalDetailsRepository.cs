using capg_hv_backend.Application.Repositories.Abstractions;
using capg_hv_backend.Domain.Entities;
using capg_hv_backend.Infrastructure.Database.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace capg_hv_backend.Application.Repositories.Internal;

public sealed class PersonalDetailsRepository(ApplicationDbContext context) : ICrudRepository<PersonalDetails>
{
    private readonly ApplicationDbContext _context = context;

    public async Task<PersonalDetails?> CreateAsync(PersonalDetails entity, CancellationToken token = default)
    {
        PersonalDetails details = (PersonalDetails)entity.Clone();
        if (details.BirthDate.Kind != DateTimeKind.Utc)
        {
            details.BirthDate = details.BirthDate.ToUniversalTime();
        }

        EntityEntry<PersonalDetails> result = await _context.PersonalDetails.AddAsync(details, token);
        await _context.SaveChangesAsync(token);

        return result?.Entity;
    }

    public async Task<PersonalDetails?> DeleteAsync(Guid userId, CancellationToken token = default)
    {
        PersonalDetails? existing = await ReadAsync(userId, token);
        if (existing is null)
        {
            return null;
        }

        _context.PersonalDetails.Entry(existing).State = EntityState.Deleted;
        await _context.SaveChangesAsync(token);

        return existing;
    }

    public async Task<PersonalDetails?> ReadAsync(Guid userId, CancellationToken token = default)
    {
        User? user = await _context.Users.FindAsync([userId], token);
        if (user is null)
        {
            return null;
        }

        await _context.Entry(user).Reference(u => u.PersonalDetails).LoadAsync(token);
        return user.PersonalDetails;
    }

    public async Task<PersonalDetails?> UpdateAsync(PersonalDetails entity, CancellationToken token = default)
    {
        PersonalDetails? existing = await ReadAsync(entity.UserId, token);
        if (existing is null)
        {
            return null;
        }

        PersonalDetails details = (PersonalDetails)entity.Clone();
        if (details.BirthDate.Kind != DateTimeKind.Utc)
        {
            details.BirthDate = details.BirthDate.ToUniversalTime();
        }

        _context.PersonalDetails.Entry(existing).CurrentValues.SetValues(details);
        await _context.SaveChangesAsync(token);

        return entity;
    }
}