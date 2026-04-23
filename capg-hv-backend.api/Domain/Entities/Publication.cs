using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace capg_hv_backend.Domain.Entities;

public sealed class Publication : BaseEntity
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

    [ForeignKey("UserId")]
    public Guid UserId { get; set; }

    public User User { get; set; } = null!;
}