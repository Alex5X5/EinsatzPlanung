namespace Einsatzplanung.Excel.Services;

using System.Collections.Generic;
using Einsatzplanung.Excel.Interfaces;
using Einsatzplanung.Excel.Models;
using Einsatzplanung.Types.Models.Configuration;
using System.Threading.Tasks;
using System;
using System.Text.RegularExpressions;
using Einsatzplanung.Types.Models;

public class GroupExportService : IConfigExportService<AgeGroupConfig> {
	
	private readonly IExcelExportService excelExportService;

	public GroupExportService(IExcelExportService excelExportService) {
		this.excelExportService = excelExportService;
	}

	public async Task ExportAsync(string filePath, List<AgeGroupConfig> ageGroups) {
		await Task.Run(() => {
			// Table table = new();
			// table.AddRow(["Lehrjahr", "Klasse", "Ausbilder", "Schulwochen", "Thema", "Wochen", "Farbe"]);

			// foreach (AgeGroupConfig agegroup in ageGroups) {
				
			// 	foreach (GroupConfig group in agegroup.Groups) {
					
			// 		foreach (var schoolWeek in group.SchoolWeeks) {
						
			// 			foreach (BlockConfig block in group.Blocks)

			// 				table.AddRow([
			// 					agegroup.Name.ToString(),
			// 					group.Name.ToString(),
			// 					group.TeacherAbbreviation.ToString(),
			// 					schoolWeek.ToString(),
			// 					block.Topic.Name.ToString(),
			// 					block.Count.ToString(),
			// 					block.Topic.Color.ToString(),
			// 				]);
			// 		}
			// 	}
			// }

		});
	}
}
