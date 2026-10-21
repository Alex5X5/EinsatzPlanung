namespace Einsatzplanung.Input.Services.Configuration;

using System;
using System.Collections.Generic;
using System.Globalization;

using Einsatzplanung.Excel.Models;
using Einsatzplanung.Excel.Interfaces;
using Einsatzplanung.Types.Models.Configuration;

public class HolidayConfigService : ConfigServiceBase<HolidayConfig> {
	
	private const int FROM_COLUMN_INDEX = 0;
	private const int TO_COLUMN_INDEX = 1;

	public HolidayConfigService(IExcelImportService importService) : base(importService) { }

	protected override List<HolidayConfig> ParseTable(Table table) {
		List<HolidayConfig> holidays = [];

		for (int row = 1; row < table.RowCount; row++) {
			if (DateTime.TryParseExact( table[row, FROM_COLUMN_INDEX]?.Value,
				"dd.MM.yyyy HH:mm:ss",
				CultureInfo.InvariantCulture,
				DateTimeStyles.None, 
				out var from)) {

				HolidayConfig config = new() {
					Id = Guid.NewGuid(),
					From = DateOnly.FromDateTime(from)
				};
				
				if (DateOnly.TryParse(table[row, TO_COLUMN_INDEX]?.Value, out var to))
					config.To = to;

				holidays.Add(config);
			}
		}

		return holidays;
	}
}
