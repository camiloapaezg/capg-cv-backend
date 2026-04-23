using System.ComponentModel.DataAnnotations;

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

    public PersonalDetails? PersonalDetails { get; set; }

    public GeneralDetails? GeneralDetails { get; set; }

    public ICollection<WorkExperience> WorkExperience { get; } = [];

    public ICollection<CertificationTraining> CertificationTraining { get; } = [];

    public ICollection<FormalEducation> Education { get; } = [];

    public ICollection<Publication> Publications { get; } = [];
}
