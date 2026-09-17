namespace Einsatzplanung.Types.Models.Configuration;

using System;

public class YearConfig {

	public required DateTime SchoolYearStart { init; get; }

	public override string ToString() {
		return $"YearConfig[]";
	}
}
