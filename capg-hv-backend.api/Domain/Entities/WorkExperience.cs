using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace capg_hv_backend.Domain.Entities;

public sealed class WorkExperience : BaseEntity
{
    [Required]
    [MaxLength(64)]
    public string JobTitle { get; set; } = null!;

    [Required]
    [MaxLength(255)]
    public string Company { get; set; } = null!;

    [Required]
    public DateTime From { get; set; }

    public DateTime? Until { get; set; }

    [Required]
    public string Description { get; set; } = null!;

    public string? ContactName { get; set; }

    [MaxLength(64)]
    public string? ContactNumber { get; set; }

    [ForeignKey("UserId")]
    public Guid UserId { get; set; }

    public User User { get; set; } = null!;
}
