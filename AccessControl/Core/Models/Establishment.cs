namespace AccessControl.Core.Models;

public partial class Establishment
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int? MaxCapacity { get; set; }

    public string? Address { get; set; }

    public string? City { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual ICollection<AccessRecord> AccessRecords { get; set; } = new List<AccessRecord>();

    public virtual ICollection<EstablishmentOpening> EstablishmentOpenings { get; set; } = new List<EstablishmentOpening>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
