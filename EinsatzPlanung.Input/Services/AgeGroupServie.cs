namespace Einsatzplanung.Input.Services;

using Einsatzplanung.Excel.Models;
using Einsatzplanung.Excel.Services;
using Einsatzplanung.Types.Models;

using Einsatzplanung.Input.Interfaces;

using System.Collections.Generic;

public class AgeGroupService : IEntityService<AgeGroup> {

	private const int AGE_GROUP_COLUMN_INDEX = 0;
	private const int CLASS_GROUP_COLUMN_INDEX = 1;
	private const int SCOOLWEEK_COLUMN_INDEX = 2;
	private const int TOPIC_COLUMN_INDEX = 3;
	private const int TOPIC_WEEKS_COLUMN_INDEX = 4;
	private const int TOPIC_COLOR_COLUMN_INDEX = 5;

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

			if (table[row, AGE_GROUP_COLUMN_INDEX]?.Value != "") {
				if (currentAgeGroup != null)
					ageGroups.Add(currentAgeGroup);
				currentAgeGroup = new AgeGroup() {
					Name = table[row, AGE_GROUP_COLUMN_INDEX]?.Value ?? "",
					Groups = []
				};
			}

			if (currentAgeGroup == null)
				continue;

			if (table[row, CLASS_GROUP_COLUMN_INDEX]?.Value != "") {
				if (currentGroup != null)
					currentAgeGroup.Groups.Add(currentGroup);
				currentGroup = new Group() {
					Name = table[row, CLASS_GROUP_COLUMN_INDEX]?.Value ?? "",
					SchoolWeeks = [],
					Blocks = []
				};
			}

			if (currentGroup == null)
				continue;

			if (int.TryParse(table[row, SCOOLWEEK_COLUMN_INDEX]?.Value ?? "", out var schoolWeek))
				currentGroup.SchoolWeeks.Add(schoolWeek);

			if ((table[row, TOPIC_COLUMN_INDEX]?.Value ?? "") != "") {
				if (int.TryParse(table[row, TOPIC_WEEKS_COLUMN_INDEX]?.Value ?? "", out var topicWeeks)) {
					string name = table[row, TOPIC_COLUMN_INDEX]!.Value!;
					string farbe = table[row, TOPIC_COLOR_COLUMN_INDEX]?.Value ?? "#FFFFFF";
					currentGroup.Blocks.Add(new Block() { Anzahl = topicWeeks, Name = name, Farbe=farbe });
				}
			}
		}
		return ageGroups;
	}
}
