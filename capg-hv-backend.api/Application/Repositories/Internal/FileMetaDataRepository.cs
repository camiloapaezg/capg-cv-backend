using capg_hv_backend.Application.Repositories.Abstractions;
using capg_hv_backend.Domain.Entities;
using capg_hv_backend.Infrastructure.Persistence.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace capg_hv_backend.Application.Repositories.Internal;

public sealed class FileMetaDataRepository(ApplicationDbContext context) : IRepository<FileMetaData>
{
    private readonly ApplicationDbContext _context = context;

    public async Task<FileMetaData?> Create(FileMetaData entity, CancellationToken token = default)
    {
        FileMetaData newEntity = (FileMetaData)entity.Clone();
        newEntity.ModifiedAt = DateTime.UtcNow;

        EntityEntry<FileMetaData> result = await _context.FileMetaData.AddAsync(newEntity, token);
        await _context.SaveChangesAsync(token);

        return result?.Entity;
    }

    public async Task<FileMetaData?> Delete(Guid id, CancellationToken token = default)
    {
        FileMetaData? existing = await Get(id, token);
        if (existing is null)
        {
            return null;
        }

        _context.FileMetaData.Entry(existing).State = EntityState.Deleted;
        await _context.SaveChangesAsync(token);

        return existing;
    }

    public async Task<bool> Exists(Guid id, CancellationToken token = default) => await _context.FileMetaData.AnyAsync(e => e.Id == id, token);

    public async Task<FileMetaData?> Get(Guid id, CancellationToken token = default) => await _context.FileMetaData.FindAsync([id], token);

    public async Task<List<FileMetaData>> List(Guid? ownerId = null, CancellationToken token = default) => await _context.FileMetaData.Where(e => e.OwnerId == ownerId).ToListAsync(token);

    public async Task<FileMetaData?> Update(FileMetaData entity, CancellationToken token = default)
    {
        FileMetaData? existing = await Get(entity.Id, token);
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