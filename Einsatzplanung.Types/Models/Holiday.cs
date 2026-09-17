using System;

namespace Einsatzplanung.Types.Models;

public class Holiday {
    public DateOnly Von { get; set; }
    public DateOnly? Bis { get; set; }
    public bool IsSingleDay => Bis == null;
    public bool IsPeriod => Bis != null;
}
