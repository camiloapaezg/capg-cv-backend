using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace capg_hv_backend.Domain.Entities;

public sealed class PersonalDetails : BaseEntity
{
    [MaxLength(64)]
    public string Nationality { get; set; } = null!;

    [MaxLength(64)]
    public string TelephoneNumber { get; set; } = null!;

    public DateTime BirthDate { get; set; }

    [ForeignKey("UserId")]
    public Guid UserId { get; set; }

    public User User { get; set; } = null!;
}
