namespace Einsatzplanung.Excel.Services;

using Einsatzplanung.Excel.Interfaces;
using Einsatzplanung.Types.Models.Configuration;
using System.Collections.Generic;

public class HolidayExportService : IHolidayExportService {
	
	private readonly IExcelExportService excelExportService;

	public HolidayExportService(IExcelExportService excelExportService) {
		this.excelExportService = excelExportService;
	}

	public void ExportHolidays(string filePath, List<HolidayConfig> holidays) {
	}
}
