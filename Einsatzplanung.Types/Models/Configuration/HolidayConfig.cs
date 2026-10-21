namespace Einsatzplanung.Types.Models.Configuration;

using System;

public class HolidayConfig : Config {
    public DateOnly From { get; set; }
    public DateOnly? To { get; set; }

    public bool IsSingleDay => To == null;
    public bool IsPeriod => To != null;
}
