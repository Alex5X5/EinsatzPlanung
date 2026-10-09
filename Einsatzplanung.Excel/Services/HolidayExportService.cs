namespace Einsatzplanung.Excel.Services;

using Einsatzplanung.Excel.Interfaces;
using Einsatzplanung.Excel.Models;
using Einsatzplanung.Types.Models.Configuration;

using System.Collections.Generic;
using System.Threading.Tasks;

public class HolidayExportService : IConfigExportService<HolidayConfig> {
	
	private readonly IExcelExportService excelExportService;

	public HolidayExportService(IExcelExportService excelExportService) {
		this.excelExportService = excelExportService;
	}

	public async Task ExportAsync(string filePath, List<HolidayConfig> holidays) {
		await Task.Run(() => {

			Table table = new();
			table.AddRow(["Von", "Bis"]);

			foreach (HolidayConfig holiday in holidays) {
				if (holiday.To is null) {
					table.AddRow([
						holiday.From.ToString("dd.MM.yyyy"),
					]);
				} else {
					table.AddRow([
						holiday.From.ToString("dd.MM.yyyy"),
						holiday.To?.ToString("dd.MM.yyyy")!
					]);
				}
			}

			excelExportService.SaveTable(filePath, table, sheetName: "Urlaub");
		});
	}
}
