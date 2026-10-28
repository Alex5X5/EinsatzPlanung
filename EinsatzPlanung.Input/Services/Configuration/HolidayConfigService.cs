namespace Einsatzplanung.Input.Services.Configuration;

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Globalization;

using Einsatzplanung.Excel.Models;
using Einsatzplanung.Excel.Interfaces;
using Einsatzplanung.Types.Models.Configuration;

public class HolidayConfigService : ConfigServiceBase<HolidayConfig> {
	
	private const int FROM_COLUMN_INDEX = 0;
	private const int TO_COLUMN_INDEX = 1;

	public HolidayConfigService(IExcelImportService importService) : base(importService) { }

	protected override async Task<List<HolidayConfig>> ParseTableAsync(Table table, CancellationToken? token = null) {
		return await Task.Run(
			async () => {
				List<HolidayConfig> holidays = [];

				for (int row = 1; row < table.RowCount; row++) {

					if (token?.IsCancellationRequested ?? false)
						break;

					var fromCell = table[row, FROM_COLUMN_INDEX];
					var toCell = table[row, TO_COLUMN_INDEX];

					if (DateTime.TryParseExact(table[row, FROM_COLUMN_INDEX]?.Value,
						"dd.MM.yyyy",
						CultureInfo.InvariantCulture,
						DateTimeStyles.None,
						out var from)) {

						HolidayConfig config = new() {
							Id = Guid.NewGuid(),
							From = DateOnly.FromDateTime(from)
						};
						if (DateTime.TryParseExact(table[row, TO_COLUMN_INDEX]?.Value,
						"dd.MM.yyyy",
						CultureInfo.InvariantCulture,
						DateTimeStyles.None,
						out var to))
							config.To = DateOnly.FromDateTime(to);

						holidays.Add(config);
					}
				}

				return holidays;
			});
	}
}
