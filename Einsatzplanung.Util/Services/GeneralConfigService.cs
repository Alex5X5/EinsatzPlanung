namespace Einsatzplanung.Util.Services;

using System;

public partial class GeneralConfigService {

	public DateTime YearStartDate { set; get; } = new(2024, 1, 1);

	public DateTime YearEndDate { set; get; } = new(2025, 1, 1);
}
