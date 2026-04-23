using System.ComponentModel.DataAnnotations;

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
}