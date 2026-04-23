using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace capg_hv_backend.Domain.Entities;

public sealed class CertificationTraining : BaseEntity
{
    [Required]
    [MaxLength(64)]
    public string Title { get; set; } = null!;

    [Required]
    [MaxLength(255)]
    public string Institution { get; set; } = null!;

    [MaxLength(255)]
    public string? CertificateNumber { get; set; }

    [Required]
    [MaxLength(64)]
    public string FinishedAt { get; set; } = null!;

    [ForeignKey("UserId")]
    public Guid UserId { get; set; }

    public User User { get; set; } = null!;
}
