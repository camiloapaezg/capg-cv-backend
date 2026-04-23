using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace capg_hv_backend.Domain.Entities;

public sealed class PersonalDetails : BaseEntity, ICloneable
{
    public DateTime BirthDate { get; set; }

    [MaxLength(64)]
    public string Nationality { get; set; } = null!;

    [MaxLength(64)]
    public string TelephoneNumber { get; set; } = null!;

    public User User { get; set; } = null!;

    [ForeignKey("UserId")]
    public Guid UserId { get; set; }

    public object Clone()
    {
        return new PersonalDetails()
        {
            Id = Id,
            Nationality = Nationality,
            TelephoneNumber = TelephoneNumber,
            BirthDate = BirthDate,
            UserId = UserId
        };
    }
}