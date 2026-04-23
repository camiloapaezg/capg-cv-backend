using System.ComponentModel.DataAnnotations;

namespace capg_hv_backend.Domain.Entities;

public sealed class FormalEducation : BaseEntity
{
    [Required]
    [MaxLength(255)]
    public string School { get; set; } = null!;

    [Required]
    [MaxLength(64)]
    public string Degree { get; set; } = null!;

    [Required]
    [MaxLength(64)]
    public string StartDate { get; set; } = null!;

    [MaxLength(64)]
    public string? EndDate { get; set; } = null!;

    public string? Description { get; set; } 
}
