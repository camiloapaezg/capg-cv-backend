using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

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
    public long SizeInBytes { get; set; }

    [JsonIgnore]
    public User User { get; set; } = null!;

    [ForeignKey("UserId")]
    public Guid UserId { get; set; }

    public object Clone()
    {
        return new FileMetaData()
        {
            Id = Id,
            Name = Name,
            SizeInBytes = SizeInBytes,
            UserId = UserId,
            Md5Hash = Md5Hash,
            ModifiedAt = ModifiedAt,
        };
    }
}