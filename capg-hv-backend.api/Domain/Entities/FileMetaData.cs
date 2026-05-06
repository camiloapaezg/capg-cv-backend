using System.ComponentModel.DataAnnotations;

namespace capg_hv_backend.Domain.Entities;

public sealed class FileMetaData : BaseEntity, ICloneable
{
    [Required]
    public string Md5Hash { get; set; } = null!;

    [Required]
    public DateTime ModifiedAt { get; set; }

    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = null!;

    [Required]
    public Guid OwnerId { get; set; }

    [Required]
    public long SizeInBytes { get; set; }

    public object Clone()
    {
        return new FileMetaData()
        {
            Id = Id,
            Name = Name,
            SizeInBytes = SizeInBytes,
            OwnerId = OwnerId,
            Md5Hash = Md5Hash,
            ModifiedAt = ModifiedAt,
        };
    }
}