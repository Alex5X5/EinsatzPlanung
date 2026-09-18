using System;

namespace Einsatzplanung.Types.Models.Configuration;

public class HolidayConfig {
    public DateOnly From { get; set; }
    public DateOnly? To { get; set; }

    public bool IsSingleDay => To == null;
    public bool IsPeriod => To != null;
}
