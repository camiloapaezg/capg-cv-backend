using capg_hv_backend.Application.Repositories.Abstractions;
using capg_hv_backend.Domain.Entities;
using capg_hv_backend.Infrastructure.Database.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace capg_hv_backend.Application.Repositories.Internal;

public sealed class FileMetaDataRepository(ApplicationDbContext context) : IListableRepository<FileMetaData>
{
    private readonly ApplicationDbContext _context = context;

    public async Task<FileMetaData?> CreateAsync(FileMetaData entity, CancellationToken token = default)
    {
        FileMetaData newEntity = (FileMetaData)entity.Clone();
        newEntity.ModifiedAt = DateTime.UtcNow;

        EntityEntry<FileMetaData> result = await _context.FileMetaData.AddAsync(newEntity, token);
        await _context.SaveChangesAsync(token);

        return result?.Entity;
    }

    public async Task<FileMetaData?> DeleteAsync(Guid id, CancellationToken token = default)
    {
        FileMetaData? existing = await ReadAsync(id, token);
        if (existing is null)
        {
            return null;
        }

        _context.FileMetaData.Entry(existing).State = EntityState.Deleted;
        await _context.SaveChangesAsync(token);

        return existing;
    }

    public async Task<List<FileMetaData>> List(Guid? userId = null, CancellationToken token = default)
    {
        User? user = await _context.Users.FindAsync([userId], token);
        if (user is null)
        {
            return [];
        }

        await _context.Entry(user).Collection(u => u.Files).LoadAsync(token);
        return [.. user.Files];
    }

    public async Task<FileMetaData?> ReadAsync(Guid id, CancellationToken token = default) => await _context.FileMetaData.FindAsync([id], token);

    public async Task<FileMetaData?> UpdateAsync(FileMetaData entity, CancellationToken token = default)
    {
        FileMetaData? existing = await ReadAsync(entity.Id, token);
        if (existing is null)
        {
            return null;
        }

        FileMetaData newEntity = (FileMetaData)entity.Clone();
        newEntity.ModifiedAt = DateTime.UtcNow;

        _context.FileMetaData.Entry(existing).CurrentValues.SetValues(newEntity);
        await _context.SaveChangesAsync(token);

        return entity;
    }
}