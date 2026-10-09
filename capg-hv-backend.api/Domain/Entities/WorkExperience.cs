using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace capg_hv_backend.Domain.Entities;

public sealed class WorkExperience : BaseEntity, ICloneable
{
    [Required]
    [MaxLength(255)]
    public string Company { get; set; } = null!;

    [MaxLength(255)]
    public string? ContactName { get; set; }

    [MaxLength(64)]
    public string? ContactNumber { get; set; }

    [Required]
    public string Description { get; set; } = null!;

    public Guid? FileId { get; set; }

    [Required]
    public DateTime From { get; set; }

    [Required]
    [MaxLength(64)]
    public string JobTitle { get; set; } = null!;

    [MaxLength(255)]
    public string Location { get; set; } = null!;

    public DateTime? Until { get; set; }

    [JsonIgnore]
    public User User { get; set; } = null!;

    [ForeignKey("UserId")]
    public Guid UserId { get; set; }

    public object Clone()
    {
        return new WorkExperience()
        {
            Id = Id,
            UserId = UserId,
            JobTitle = JobTitle,
            Company = Company,
            From = From,
            Until = Until,
            ContactName = ContactName,
            ContactNumber = ContactNumber,
            Description = Description,
            Location = Location,
            FileId = FileId,
        };
    }
}