using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace capg_hv_backend.Domain.Entities;

public sealed class GeneralDetails : BaseEntity
{
    [MaxLength(64)]
    public string Title { get; set; } = null!;

    [MaxLength(64)]
    public string? Description { get; set; }

    public string? TechnicalSkills { get; set; }

    public string? Languages { get; set; }

    [ForeignKey("UserId")]
    public Guid UserId { get; set; }

    public User User { get; set; } = null!;
}
