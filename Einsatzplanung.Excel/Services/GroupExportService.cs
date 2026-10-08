namespace Einsatzplanung.Excel.Services;

using System.Collections.Generic;
using Einsatzplanung.Types.Models.Configuration;
using Einsatzplanung.Excel.Interfaces;

public class GroupExportService : IConfigExportService<GroupConfig> {
	
	private readonly IExcelExportService excelExportService;

	public GroupExportService(IExcelExportService excelExportService) {
		this.excelExportService = excelExportService;
	}

	public void Export(string filePath, List<GroupConfig> groups) {
	}
}
