namespace Einsatzplanung.Excel.Services;

using Einsatzplanung.Excel.Interfaces;
using Einsatzplanung.Types.Models;
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
		});
	}
}
