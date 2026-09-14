namespace EinsatzPlanung.Input.Services;

using Einsatzplanung.Excel.Models;
using Einsatzplanung.Excel.Services;
using Einsatzplanung.Types.Models;

using EinsatzPlanung.Input.Interfaces;

using System.Collections.Generic;

public class AgeGroupService : IEntityService<AgeGroup> {

	private const int NAME_COLUMN_INDEX = 0;
	private const int ABBREVIATION_COLUMN_INDEX = 1;
	private const int SPECIALIZATION_COLUMN_INDEX = 2;
	private const int WEEKLY_HOURS_COLUMN_INDEX = 3;

	private ExcelImportService excelImportService;

	private string SourceFilePath { get; set; } = "";

	public AgeGroupService(ExcelImportService excelImportService) {
		this.excelImportService = excelImportService;
	}

	public void SetSource(string path) {
		SourceFilePath = path;
	}

	public List<AgeGroup> ParseSource() {
		//Table table = excelImportService.GetTable(SourceFilePath);
		//List<AgeGroup> ageGroups = [];
		//AgeGroup? group = null;
		//for (int row = 1; row < table.RowCount; row++) {
		//	if (table[row, ABBREVIATION_COLUMN_INDEX]?.Value != "") {
		//		if (group != null)
		//			ageGroups.Add(group);
		//		group = new() {
		//			Name = table[row, NAME_COLUMN_INDEX]?.Value ?? "",
		//			Abbreviation = table[row, ABBREVIATION_COLUMN_INDEX]?.Value ?? "",
		//			WeeklyHours = int.Parse(table[row, WEEKLY_HOURS_COLUMN_INDEX]?.Value ?? "0"),
		//			Specializations = []
		//		};
		//	}
		//	if (group == null)
		//		continue;
		//	group.Specializations.Add(new Topic(table[row, SPECIALIZATION_COLUMN_INDEX]?.Value ?? ""));
		//}
		//return ageGroups;
		return [];
	}
}
