using capg_hv_backend.Application.Repositories.Abstractions;
using capg_hv_backend.Domain.Entities;
using capg_hv_backend.Infrastructure.Database.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace capg_hv_backend.Application.Repositories.Internal;

public sealed class UsersRepository(ApplicationDbContext context) : IListableRepository<User>
{
    private readonly ApplicationDbContext _context = context;

    public async Task<User?> CreateAsync(User newUser, CancellationToken token = default)
    {
        EntityEntry<User> result = await _context.Users.AddAsync(newUser, token);
        await _context.SaveChangesAsync(token);

        return result?.Entity;
    }

    public async Task<User?> DeleteAsync(Guid id, CancellationToken token = default)
    {
        User? existing = await ReadAsync(id, token);
        if (existing is null)
        {
            return null;
        }

        _context.Users.Entry(existing).State = EntityState.Deleted;
        await _context.SaveChangesAsync(token);

        return existing;
    }

    public async Task<List<User>> List(Guid? id = null, CancellationToken token = default) => await _context.Users.ToListAsync(token);

    public async Task<User?> ReadAsync(Guid id, CancellationToken token = default) => await _context.Users.FindAsync([id], token);

    public async Task<User?> UpdateAsync(User newUser, CancellationToken token = default)
    {
        User? existing = await ReadAsync(newUser.Id, token);
        if (existing is null)
        {
            return null;
        }

        _context.Users.Entry(existing).CurrentValues.SetValues(newUser);
        await _context.SaveChangesAsync(token);

        return newUser;
    }
}