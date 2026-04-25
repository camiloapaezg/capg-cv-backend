using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace capg_hv_backend.Domain.Entities;

public sealed class User : BaseEntity
{
    [Required]
    [MaxLength(64)]
    public string FirstName { get; set; } = null!;

    [Required]
    [MaxLength(64)]
    public string LastName { get; set; } = null!;

    [Required]
    [MaxLength(64)]
    public string EmailAddress { get; set; } = null!;

    [JsonIgnore]
    public PersonalDetails? PersonalDetails { get; set; }

    [JsonIgnore]
    public GeneralDetails? GeneralDetails { get; set; }

    [JsonIgnore]
    public ICollection<WorkExperience> WorkExperience { get; } = [];

    [JsonIgnore]
    public ICollection<CertificationTraining> CertificationTraining { get; } = [];

    [JsonIgnore]
    public ICollection<FormalEducation> Education { get; } = [];

    [JsonIgnore]
    public ICollection<Publication> Publications { get; } = [];
}
