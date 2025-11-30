namespace AccessControl.Core.Models;

public partial class User
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public int EstablishmentId { get; set; }

    public string Role { get; set; } = null!;

    public bool? IsActive { get; set; }

    public string IdentityDocument { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public bool? MustChangePassword { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? LastLoginDate { get; set; }

    public virtual ICollection<AccessRecord> AccessRecords { get; set; } = new List<AccessRecord>();

    public virtual Establishment Establishment { get; set; } = null!;

    public virtual ICollection<EstablishmentOpening> EstablishmentOpenings { get; set; } = new List<EstablishmentOpening>();
}
