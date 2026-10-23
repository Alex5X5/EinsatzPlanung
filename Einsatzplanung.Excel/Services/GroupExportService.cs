namespace Einsatzplanung.Excel.Services;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Einsatzplanung.Excel.Interfaces;
using Einsatzplanung.Excel.Models;
using Einsatzplanung.Types.Models.Configuration;

public class GroupExportService : IConfigExportService<AgeGroupConfig> {
	
	private readonly IExcelExportService excelExportService;

	public GroupExportService(IExcelExportService excelExportService) {
		this.excelExportService = excelExportService;
	}

	public async Task ExportAsync(string filePath, List<AgeGroupConfig> ageGroups) {
		await Task.Run(() => {
			Table table = new();
			table.AddRow(["Lehrjahr", "Klasse", "Ausbilder", "Schulwochen", "Thema", "Wochen", "Farbe"]);

			bool isAgeGroupRow = false;
			bool isGroupRow = false;

			foreach (AgeGroupConfig agegroup in ageGroups) {
				isAgeGroupRow = true;

				foreach (GroupConfig group in agegroup.Groups) {
					isGroupRow = true;

					int rows = Math.Max(group.Blocks.Count, group.SchoolWeeks.Count);

					for (int i = 0; i < rows; i++) {
						bool isSchoolRow = group.SchoolWeeks.Count > i;
						bool isBlockRow = group.Blocks.Count > i;

						table.AddRow([
							isAgeGroupRow ? agegroup.Name.ToString() : "",
							isGroupRow ? group.Name.ToString() : "",
							isGroupRow ? group.Teacher.Abbreviation : "",
							isSchoolRow ? group.SchoolWeeks[i].ToString() : "",
				 			isBlockRow ? group.Blocks[i].Topic.Name.ToString() : "",
				 			isBlockRow ? group.Blocks[i].Count.ToString() : ""
						]);

						isAgeGroupRow = false;
						isGroupRow = false;
					}

				}
			}

			excelExportService.SaveTable(filePath, table);
		});
	}
}
