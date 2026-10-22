using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace capg_hv_backend.Domain.Entities;

public sealed class User : BaseEntity, ICloneable
{
    [JsonIgnore]
    public ICollection<CertificationTraining> CertificationTraining { get; } = [];

    [JsonIgnore]
    public ICollection<FormalEducation> Education { get; } = [];

    [Required]
    [MaxLength(64)]
    public string EmailAddress { get; set; } = null!;

    [JsonIgnore]
    public ICollection<FileMetaData> Files { get; } = [];

    [Required]
    [MaxLength(64)]
    public string FirstName { get; set; } = null!;

    [JsonIgnore]
    public GeneralDetails? GeneralDetails { get; set; }

    [Required]
    [MaxLength(64)]
    public string LastName { get; set; } = null!;

    [JsonIgnore]
    public PersonalDetails? PersonalDetails { get; set; }
    [JsonIgnore]
    public ICollection<Publication> Publications { get; } = [];

    [JsonIgnore]
    public ICollection<WorkExperience> WorkExperience { get; } = [];

    public object Clone()
    {
        return new User()
        {
            Id = Id,
            EmailAddress = EmailAddress,
            FirstName = FirstName,
            LastName = LastName,
        };
    }
}