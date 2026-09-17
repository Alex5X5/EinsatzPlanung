using System;

namespace Einsatzplanung.Types.Models.Configuration;

public class HolidayConfig {
    public DateOnly Von { get; set; }
    public DateOnly? Bis { get; set; }

    public bool IsSingleDay => Bis == null;
    public bool IsPeriod => Bis != null;
}
