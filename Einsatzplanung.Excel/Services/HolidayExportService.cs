namespace Einsatzplanung.Excel.Services;

using Einsatzplanung.Excel.Interfaces;
using Einsatzplanung.Types.Models.Configuration;
using System.Collections.Generic;

public class HolidayExportService : IConfigExportService<HolidayConfig> {
	
	private readonly IExcelExportService excelExportService;

	public HolidayExportService(IExcelExportService excelExportService) {
		this.excelExportService = excelExportService;
	}

	public void Export(string filePath, List<HolidayConfig> holidays) {
	}
}
