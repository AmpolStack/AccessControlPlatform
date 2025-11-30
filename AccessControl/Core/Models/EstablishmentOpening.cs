namespace AccessControl.Core.Models;

public partial class EstablishmentOpening
{
    public int Id { get; set; }

    public int EstablishmentId { get; set; }

    public int UserId { get; set; }

    public DateTime OpeningDateTime { get; set; }

    public DateTime? ClosingDateTime { get; set; }

    public string? Status { get; set; }

    public virtual Establishment Establishment { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
