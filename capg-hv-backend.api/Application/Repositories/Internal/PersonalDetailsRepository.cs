using capg_hv_backend.Application.Repositories.Abstractions;
using capg_hv_backend.Domain.Entities;
using capg_hv_backend.Infrastructure.Persistence.Internal;
using Microsoft.EntityFrameworkCore;

namespace capg_hv_backend.Application.Repositories.Internal;

public sealed class PersonalDetailsRepository(ApplicationDbContext context) : IRepository<PersonalDetails>
{
    private readonly ApplicationDbContext _context = context;

    public async Task<PersonalDetails?> Create(PersonalDetails entity, CancellationToken token = default)
    {
        PersonalDetails details = (PersonalDetails)entity.Clone();
        if (details.BirthDate.Kind != DateTimeKind.Utc)
        {
            details.BirthDate = details.BirthDate.ToUniversalTime();
        }

        var result = await _context.PersonalDetails.AddAsync(details, token);
        await _context.SaveChangesAsync(token);

        return result?.Entity;
    }

    public async Task<PersonalDetails?> Delete(Guid id, CancellationToken token = default)
    {
        var existing = await Get(id, token);
        if (existing is null)
        {
            return null;
        }

        _context.PersonalDetails.Entry(existing).State = EntityState.Deleted;
        await _context.SaveChangesAsync(token);

        return existing;
    }

    public async Task<bool> Exists(Guid id, CancellationToken token = default) => await _context.PersonalDetails.AnyAsync(e => e.Id == id, token);

    public async Task<PersonalDetails?> Get(Guid id, CancellationToken token = default) => await _context.PersonalDetails.FindAsync([id], token);

    public async Task<List<PersonalDetails>> List(Guid? userId = null) => await _context.PersonalDetails.Where(i => i.UserId == userId).ToListAsync();

    public async Task<PersonalDetails?> Update(PersonalDetails entity, CancellationToken token = default)
    {
        var existing = await Get(entity.Id, token);
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