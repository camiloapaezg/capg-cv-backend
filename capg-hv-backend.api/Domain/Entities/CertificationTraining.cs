using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace capg_hv_backend.Domain.Entities;

public sealed class CertificationTraining : BaseEntity, ICloneable
{
    [MaxLength(255)]
    public string? CertificateNumber { get; set; }

    public Guid? FileId { get; set; }

    [Required]
    [MaxLength(64)]
    public string FinishedAt { get; set; } = null!;

    [Required]
    [MaxLength(255)]
    public string Institution { get; set; } = null!;

    [Required]
    [MaxLength(64)]
    public string Title { get; set; } = null!;

    [JsonIgnore]
    public User User { get; set; } = null!;

    [ForeignKey("UserId")]
    public Guid UserId { get; set; }

    public object Clone()
    {
        return new CertificationTraining()
        {
            Id = Id,
            Title = Title,
            Institution = Institution,
            CertificateNumber = CertificateNumber,
            FinishedAt = FinishedAt,
            UserId = UserId,
            FileId = FileId,
        };
    }
}