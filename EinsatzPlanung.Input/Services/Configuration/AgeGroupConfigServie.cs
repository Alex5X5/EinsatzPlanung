namespace Einsatzplanung.Input.Services.Configuration;

using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

using Einsatzplanung.Excel.Models;
using Einsatzplanung.Excel.Interfaces;
using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models.Configuration;

public class AgeGroupConfigService : ConfigServiceBase<AgeGroupConfig> {

	private const int AGE_GROUP_COLUMN_INDEX = 0;
	private const int CLASS_GROUP_COLUMN_INDEX = 1;
	private const int Teacher_COLUMN_INDEX = 2;
	private const int SCOOLWEEK_COLUMN_INDEX = 3;
	private const int TOPIC_COLUMN_INDEX = 4;
	private const int TOPIC_WEEKS_COLUMN_INDEX = 5;

	private IConfigService<TopicConfig> topicService;

	public AgeGroupConfigService(IExcelImportService importService, IConfigService<TopicConfig> topicService) : base(importService) {
		this.topicService = topicService;
	}

	protected override List<AgeGroupConfig> ParseTable(Table table) {
		List<TopicConfig> topics = topicService.GetData();
		List<AgeGroupConfig> ageGroups = [];

		AgeGroupBuilder ageGroupBuilder = new();
		GroupConfigurationBuilder groupBuilder = new();

		for (int row = 1; row < table.RowCount; row++) {

			bool newAgeGroup = table[row, AGE_GROUP_COLUMN_INDEX]?.Value != "";
			bool newGroup = table[row, CLASS_GROUP_COLUMN_INDEX]?.Value != "";

			// the age group cell is not empty
			if (newAgeGroup) {
				// try to build the current group and add it to the current age group
				ageGroupBuilder.AddGroup(groupBuilder.Build());
				groupBuilder = new();
				groupBuilder.SetName(table[row, CLASS_GROUP_COLUMN_INDEX]?.Value ?? "");
				groupBuilder.SetTeacher(table[row, Teacher_COLUMN_INDEX]?.Value ?? "");
				// try to build the current age group and add it to the list of age groups
				if (ageGroupBuilder.Build() is AgeGroupConfig group)
					ageGroups.Add(group);
				ageGroupBuilder = new();
				ageGroupBuilder.SetName(table[row, AGE_GROUP_COLUMN_INDEX]?.Value ?? "");
				groupBuilder.SetAgeGroupName(ageGroupBuilder.Build()?.Name ?? "");
			} else if (newGroup) {
				// build only the current group and add it to the current age group
				ageGroupBuilder.AddGroup(groupBuilder.Build());
				groupBuilder = new();
				groupBuilder.SetAgeGroupName(ageGroupBuilder.Build()?.Name ?? "");
				groupBuilder.SetName(table[row, CLASS_GROUP_COLUMN_INDEX]?.Value ?? "");
				groupBuilder.SetTeacher(table[row, Teacher_COLUMN_INDEX]?.Value ?? "");
			}

			if (int.TryParse(table[row, SCOOLWEEK_COLUMN_INDEX]?.Value ?? "", out var schoolWeek))
				groupBuilder.AddSchoolWeek(schoolWeek);

			if (int.TryParse(table[row, TOPIC_WEEKS_COLUMN_INDEX]?.Value ?? "", out var topicWeeks)) {
				string topicName = table[row, TOPIC_COLUMN_INDEX]?.Value ?? "";
				var topic = topicService.GetData().FirstOrDefault(x => x.Name == topicName);
				if (topic != null)
					groupBuilder.AddBlock(new BlockConfig() {
						Id = Guid.NewGuid(),
						Count = topicWeeks,
						Topic = topic
					});
			}
		}

		ageGroupBuilder.AddGroup(groupBuilder.Build());
		if (ageGroupBuilder.Build() is AgeGroupConfig ageGroup)
			ageGroups.Add(ageGroup);

		return ageGroups;
	}

	protected override async Task<List<AgeGroupConfig>> UpdateReferencesAsync(List<AgeGroupConfig> data) {
		return await Task.Run(
			() => {
				return data
					.Select(ageGroup =>
						new AgeGroupConfig() {
							Id = ageGroup.Id,
							Name = ageGroup.Name,
							Groups = ageGroup.Groups
								.Select(group =>
									new GroupConfig() {
										Id = group.Id,
										Name = group.Name,
										AgeGroupName = ageGroup.Name,
										TeacherAbbreviation = group.TeacherAbbreviation,
										SchoolWeeks = group.SchoolWeeks,
										Blocks = group.Blocks
											.Select(block =>
												new BlockConfig() {
													Id = block.Id,
													Count = block.Count,
													Topic = topicService.GetById(block.Topic.Id) ?? TopicConfig.Default
												})
											.ToList()

									})
								.ToList()
						})
					.ToList();
			}
		);
	}

	private class AgeGroupBuilder {

		private AgeGroupConfig? current;

		private void CreateCurrentIfNull() {
			current ??= new AgeGroupConfig() {
				Id = Guid.NewGuid(),
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
				Id = Guid.NewGuid(),
				Name = "",
				AgeGroupName = "",
				SchoolWeeks = [],
				TeacherAbbreviation = "",
				Blocks = []
			};
		}

		public GroupConfigurationBuilder SetName(string name) {
			CreateCurrentIfNull();
			current!.Name = name;
			return this;
		}

		public GroupConfigurationBuilder SetAgeGroupName(string name) {
			CreateCurrentIfNull();
			current!.AgeGroupName = name;
			return this;
		}

		public GroupConfigurationBuilder SetTeacher(string abbreviation) {
			CreateCurrentIfNull();
			current!.TeacherAbbreviation = abbreviation;
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
