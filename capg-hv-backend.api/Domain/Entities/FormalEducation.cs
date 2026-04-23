using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace capg_hv_backend.Domain.Entities;

public sealed class FormalEducation : BaseEntity, ICloneable
{
    [Required]
    [MaxLength(64)]
    public string Degree { get; set; } = null!;

    public string? Description { get; set; }

    [MaxLength(64)]
    public string? EndDate { get; set; } = null!;

    [Required]
    [MaxLength(255)]
    public string School { get; set; } = null!;

    [Required]
    [MaxLength(64)]
    public string StartDate { get; set; } = null!;

    public User User { get; set; } = null!;

    [ForeignKey("UserId")]
    public Guid UserId { get; set; }

    public object Clone()
    {
        return new FormalEducation()
        {
            Id = Id,
            School = School,
            Degree = Degree,
            StartDate = StartDate,
            EndDate = EndDate,
            Description = Description,
            UserId = UserId
        };
    }
}