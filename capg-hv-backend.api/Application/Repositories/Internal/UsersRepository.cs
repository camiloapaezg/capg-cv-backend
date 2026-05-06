using capg_hv_backend.Application.Repositories.Abstractions;
using capg_hv_backend.Domain.Entities;
using capg_hv_backend.Infrastructure.Persistence.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace capg_hv_backend.Application.Repositories.Internal;

public sealed class UsersRepository(ApplicationDbContext context) : IRepository<User>
{
    private readonly ApplicationDbContext _context = context;

    public async Task<User?> Create(User newUser, CancellationToken token = default)
    {
        EntityEntry<User> result = await _context.Users.AddAsync(newUser, token);
        await _context.SaveChangesAsync(token);

        return result?.Entity;
    }

    public async Task<User?> Delete(Guid id, CancellationToken token = default)
    {
        User? existing = await Get(id, token);
        if (existing is null)
        {
            return null;
        }

        _context.Users.Entry(existing).State = EntityState.Deleted;
        await _context.SaveChangesAsync(token);

        return existing;
    }

    public async Task<bool> Exists(Guid id, CancellationToken token = default) => await _context.Users.AnyAsync(e => e.Id == id, token);

    public async Task<User?> Get(Guid id, CancellationToken token = default) => await _context.Users.FindAsync([id], token);

    public async Task<List<User>> List(Guid? id = null) => await _context.Users.ToListAsync();

    public async Task<User?> Update(User newUser, CancellationToken token = default)
    {
        User? existing = await Get(newUser.Id, token);
        if (existing is null)
        {
            return null;
        }

        _context.Users.Entry(existing).CurrentValues.SetValues(newUser);
        await _context.SaveChangesAsync(token);

        return newUser;
    }
}