namespace Einsatzplanung.Excel.Services;

using Einsatzplanung.Excel.Interfaces;
using Einsatzplanung.Excel.Models;
using Einsatzplanung.Types.Models.Configuration;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

public class TopicExportService : IConfigExportService<TopicConfig> {

	private readonly IExcelExportService excelExportService;

	public TopicExportService(IExcelExportService excelExportService) {
		this.excelExportService = excelExportService;
	}

	public async Task ExportAsync(string filePath, List<TopicConfig> topics) {
		await Task.Run(() => {

			Table table = new();
			table.AddRow(["Thema", "Farbe", "Level"]);

			foreach (TopicConfig topic in topics) {
				table.AddRow([
					topic.Name,
					topic.Color,
					topic.Level
				]);
			}

			excelExportService.SaveTable(filePath, table, sheetName: "Themen");
		});
	}
}
