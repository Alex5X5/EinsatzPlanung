namespace EinsatzPlanung.Input.Services;

using Einsatzplanung.Excel.Models;
using Einsatzplanung.Excel.Services;
using Einsatzplanung.Types.Models;

using EinsatzPlanung.Input.Interfaces;

using System.Collections.Generic;

public class AgeGroupService : IEntityService<AgeGroup> {

	private const int AGE_COLUMN_INDEX = 0;
	private const int CLASS_COLUMN_INDEX = 1;
	private const int SCOOLWEEK_COLUMN_INDEX = 2;

	private ExcelImportService excelImportService;

	private string SourceFilePath { get; set; } = "";

	public AgeGroupService(ExcelImportService excelImportService) {
		this.excelImportService = excelImportService;
	}

	public void SetSource(string path) {
		SourceFilePath = path;
	}

	public List<AgeGroup> ParseSource() {
		Table table = excelImportService.GetTable(SourceFilePath);
		List<AgeGroup> ageGroups = [];
		AgeGroup? currentAgeGroup = null;
		Group? currentGroup = null;

		for (int row = 1; row < table.RowCount; row++) {

			if (table[row, AGE_COLUMN_INDEX]?.Value != "") {

				if (currentAgeGroup != null)
					ageGroups.Add(currentAgeGroup);
				currentAgeGroup = new AgeGroup() {
					Name = table[row, AGE_COLUMN_INDEX]?.Value ?? "",
					Groups = []
				};
			}

			if (table[row, CLASS_COLUMN_INDEX]?.Value != "") {

				if (currentGroup != null)
					currentAgeGroup.Groups.Add(currentGroup);
				currentGroup = new Group() {
					Name = table[row, CLASS_COLUMN_INDEX]?.Value ?? "",
					SchoolWeeks = []
				};
			}

			currentGroup.SchoolWeeks.Add(int.Parse(table[row, SCOOLWEEK_COLUMN_INDEX]?.Value ?? "1"));

			
		}
		return ageGroups;
	}
}
