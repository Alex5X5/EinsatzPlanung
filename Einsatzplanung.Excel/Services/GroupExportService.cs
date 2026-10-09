namespace Einsatzplanung.Excel.Services;

using System.Collections.Generic;
using Einsatzplanung.Types.Models.Configuration;
using Einsatzplanung.Excel.Interfaces;
using System.Threading.Tasks;

public class GroupExportService : IConfigExportService<GroupConfig> {
	
	private readonly IExcelExportService excelExportService;

	public GroupExportService(IExcelExportService excelExportService) {
		this.excelExportService = excelExportService;
	}

	public async Task ExportAsync(string filePath, List<GroupConfig> groups) {
		await Task.Run(() => {
		});
	}
}
