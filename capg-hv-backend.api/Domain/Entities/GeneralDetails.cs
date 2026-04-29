using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace capg_hv_backend.Domain.Entities;

public sealed class GeneralDetails : BaseEntity, ICloneable
{
    [MaxLength(64)]
    public string? Description { get; set; }

    public string? Languages { get; set; }

    public string? TechnicalSkills { get; set; }

    [Required]
    [MaxLength(64)]
    public string Title { get; set; } = null!;

    [JsonIgnore]
    public User User { get; set; } = null!;

    [ForeignKey("UserId")]
    public Guid UserId { get; set; }

    public object Clone()
    {
        return new GeneralDetails()
        {
            Id = Id,
            Title = Title,
            Description = Description,
            TechnicalSkills = TechnicalSkills,
            Languages = Languages,
            UserId = UserId,
        };
    }
}