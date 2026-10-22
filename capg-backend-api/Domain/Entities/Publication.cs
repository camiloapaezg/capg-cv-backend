using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace capg_hv_backend.Domain.Entities;

public sealed class Publication : BaseEntity, ICloneable
{
    public string? Doi { get; set; }

    [Required]
    public string Location { get; set; } = null!;

    [Required]
    public string PublishedAt { get; set; } = null!;

    [Required]
    public string Title { get; set; } = null!;

    [Required]
    public string Type { get; set; } = null!;

    [JsonIgnore]
    public User User { get; set; } = null!;

    [ForeignKey("UserId")]
    public Guid UserId { get; set; }

    public object Clone()
    {
        return new Publication()
        {
            Id = Id,
            UserId = UserId,
            Title = Title,
            Type = Type,
            PublishedAt = PublishedAt,
            Doi = Doi,
            Location = Location,
        };
    }
}