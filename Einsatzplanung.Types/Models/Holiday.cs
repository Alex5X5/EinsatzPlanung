namespace Einsatzplanung.Types.Models;

using System;

public class Holiday {
    public DateOnly From { get; set; }
    public DateOnly To { get; set; }
    public bool IsSingleDay => From == To;
    public bool IsPeriod => From != To;

	public override string ToString() {
		return $"Holiday[From={From}, To={To}]";
	}
}
