namespace Einsatzplanung.Input.Services.Config;

using Einsatzplanung.Excel.Models;
using Einsatzplanung.Excel.Services;
using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models.Configuration;

using System.Collections.Generic;

public class AgeGroupConfigService : IConfigService<AgeGroupConfig> {

	private const int AGE_GROUP_COLUMN_INDEX = 0;
	private const int CLASS_GROUP_COLUMN_INDEX = 1;
	private const int SCOOLWEEK_COLUMN_INDEX = 2;
	private const int TOPIC_COLUMN_INDEX = 3;
	private const int TOPIC_WEEKS_COLUMN_INDEX = 4;
	private const int TOPIC_COLOR_COLUMN_INDEX = 5;

	private ExcelImportService excelImportService;

	private string SourceFilePath { get; set; } = "";

	public AgeGroupConfigService(ExcelImportService excelImportService) {
		this.excelImportService = excelImportService;
	}

	public void SetSource(string path) {
		SourceFilePath = path;
	}

	public List<AgeGroupConfig> ParseSource() {
		Table table = excelImportService.GetTable(SourceFilePath);

		List<AgeGroupConfig> ageGroups = [];

		AgeGroupBuilder ageGroupBuilder = new();
		GroupConfigurationBuilder groupBuilder = new();

		for (int row = 1; row < table.RowCount; row++) {

			bool newAgeGroup = table[row, AGE_GROUP_COLUMN_INDEX]?.Value != "";
			bool newGroup = table[row, CLASS_GROUP_COLUMN_INDEX]?.Value != "";
			bool newTopic = table[row, TOPIC_COLUMN_INDEX]?.Value != "";

			// the age group cell is not empty
			if (newAgeGroup) {
				// try to build the current group and add it to the current age group
				ageGroupBuilder.AddGroup(groupBuilder.Build());
				groupBuilder = new();
				groupBuilder.SetName(table[row, CLASS_GROUP_COLUMN_INDEX]?.Value ?? "");
				// try to build the current age group and add it to the list of age groups
				if (ageGroupBuilder.Build() is AgeGroupConfig group)
					ageGroups.Add(group);
				ageGroupBuilder = new();
				ageGroupBuilder.SetName(table[row, AGE_GROUP_COLUMN_INDEX]?.Value ?? "");
			} else if (newGroup) {
				// build only the current group and add it to the current age group
				ageGroupBuilder.AddGroup(groupBuilder.Build());
				groupBuilder = new();
				groupBuilder.SetName(table[row, CLASS_GROUP_COLUMN_INDEX]?.Value ?? "");
			}
			
			if (int.TryParse(table[row, SCOOLWEEK_COLUMN_INDEX]?.Value ?? "", out var schoolWeek))
				groupBuilder.AddSchoolWeek(schoolWeek);
			
			if (int.TryParse(table[row, TOPIC_WEEKS_COLUMN_INDEX]?.Value ?? "", out var topicWeeks)) {
				string name = table[row, TOPIC_COLUMN_INDEX]!.Value!;
				string farbe = table[row, TOPIC_COLOR_COLUMN_INDEX]?.Value ?? "#FFFFFF";
				groupBuilder.AddBlock(new BlockConfig() { Anzahl = topicWeeks, Name = name, Color = farbe });
			}
		}

		ageGroupBuilder.AddGroup(groupBuilder.Build());
		ageGroups.Add(ageGroupBuilder.Build());

		return ageGroups;
	}

	private class AgeGroupBuilder {

		private AgeGroupConfig? current;

		private void CreateCurrentIfNull() {
			current ??= new AgeGroupConfig() {
				Name = "",
				Groups = []
			};
		}

		public AgeGroupBuilder SetName(string name) {
			CreateCurrentIfNull();
			current!.Name = name;
			return this;
		}

		public AgeGroupBuilder AddGroup(GroupConfig? group) {
			if (group == null)
				return this;
			CreateCurrentIfNull();
			current!.Groups.Add(group);
			return this;
		}

		public AgeGroupConfig? Build() {
			return current;
		}
	}

	private class GroupConfigurationBuilder {
		
		private GroupConfig? current;

		private void CreateCurrentIfNull() {
			current ??= new GroupConfig() {
				Name = "",
				SchoolWeeks = [],
				Blocks = []
			};
		}

		public GroupConfigurationBuilder SetName(string name) {
			CreateCurrentIfNull();
			current!.Name = name;
			return this;
		}

		public GroupConfigurationBuilder AddSchoolWeek(int week) {
			CreateCurrentIfNull();
			current!.SchoolWeeks.Add(week);
			return this;
		}

		public GroupConfigurationBuilder AddBlock(BlockConfig block) {
			CreateCurrentIfNull();
			current!.Blocks.Add(block);
			return this;
		}

		public GroupConfig? Build() {
			return current;
		}
	}
}
