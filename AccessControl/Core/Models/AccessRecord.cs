using System;
using System.Collections.Generic;

namespace AccessControl.Models;

public partial class AccessRecord
{
    public long Id { get; set; }

    public int UserId { get; set; }

    public int EstablishmentId { get; set; }

    public DateTime EntryDateTime { get; set; }

    public DateTime? ExitDateTime { get; set; }

    public virtual Establishment Establishment { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
