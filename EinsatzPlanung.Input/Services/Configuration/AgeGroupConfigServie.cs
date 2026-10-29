namespace Einsatzplanung.Input.Services.Configuration;

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using Einsatzplanung.Excel.Interfaces;
using Einsatzplanung.Excel.Models;
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
	private IConfigService<TeacherConfig> teacherService;

	public AgeGroupConfigService(IExcelImportService importService, IConfigService<TopicConfig> topicService, IConfigService<TeacherConfig> teacherService) : base(importService) {
		this.topicService = topicService;
		this.teacherService = teacherService;
	}

	protected override async Task<List<AgeGroupConfig>> ParseTableAsync(Table table, CancellationToken? token = null) {
		List<TopicConfig> topics = await topicService.GetDataAsync(token);
		List<AgeGroupConfig> ageGroups = [];

		AgeGroupBuilder ageGroupBuilder = new();
		GroupConfigurationBuilder groupBuilder = new();

		for (int row = 1; row < table.RowCount; row++) {

			if (token?.IsCancellationRequested ?? false)
				break;

			bool newAgeGroup = table[row, AGE_GROUP_COLUMN_INDEX]?.Value != "";
			bool newGroup = table[row, CLASS_GROUP_COLUMN_INDEX]?.Value != "";

			// the age group cell is not empty
			if (newAgeGroup) {
				// try to build the current group and add it to the current age group
				ageGroupBuilder.AddGroup(groupBuilder.Build());
				groupBuilder = new();
				groupBuilder.SetName(table[row, CLASS_GROUP_COLUMN_INDEX]?.Value ?? "");
				var teacherAbbreviation = table[row, Teacher_COLUMN_INDEX]?.Value ?? "";
				var teacher = (await teacherService.GetByAbbreviationAsync(teacherAbbreviation)) ?? TeacherConfig.Default;
				groupBuilder.SetTeacher(teacher);
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
				var teacherAbbreviation = table[row, Teacher_COLUMN_INDEX]?.Value ?? "";
				var teacher = (await teacherService.GetByAbbreviationAsync(teacherAbbreviation)) ?? TeacherConfig.Default;
				groupBuilder.SetTeacher(teacher);
			}

			if (int.TryParse(table[row, SCOOLWEEK_COLUMN_INDEX]?.Value ?? "", out var schoolWeek))
				groupBuilder.AddSchoolWeek(schoolWeek);

			if (int.TryParse(table[row, TOPIC_WEEKS_COLUMN_INDEX]?.Value ?? "", out var topicWeeks)) {
				string topicName = table[row, TOPIC_COLUMN_INDEX]?.Value ?? "";
				var topic = await topicService.TryGetByNameAsync(topicName);
				if (topic.Success)
					groupBuilder.AddBlock(new BlockConfig() {
						Id = Guid.NewGuid(),
						Count = topicWeeks,
						Topic = topic.Value!
					});
			}
		}

		ageGroupBuilder.AddGroup(groupBuilder.Build());
		if (ageGroupBuilder.Build() is AgeGroupConfig ageGroup)
			ageGroups.Add(ageGroup);

		return ageGroups;
	}

	protected override async Task<List<AgeGroupConfig>> UpdateReferencesAsync(List<AgeGroupConfig> data, CancellationToken? token = null) {
		return await Task.Run(
			async () => {
				List<AgeGroupConfig> ageGroups = [];
				foreach (var oldAgeGroup in data) {

					List<GroupConfig> groups = [];
					foreach (var oldGroup in oldAgeGroup.Groups) {

						List<BlockConfig> blocks = [];
						foreach (var oldBlock in oldGroup.Blocks) {
							var block = new BlockConfig() {
								Id = oldBlock.Id,
								Count = oldBlock.Count,
								Topic = await topicService.GetByIdAsync(oldBlock.Topic.Id) ?? TopicConfig.Default,
							};

							blocks.Add(block);

							if (token?.IsCancellationRequested ?? false)
								break;
						}

						var group = new GroupConfig() {
							Id = oldGroup.Id,
							Name = oldGroup.Name,
							Teacher = (await teacherService.GetByIdAsync(oldGroup.Teacher.Id)) ?? TeacherConfig.Default,
							SchoolWeeks = oldGroup.SchoolWeeks,
							Blocks = blocks
						};

						groups.Add(group);

						if (token?.IsCancellationRequested ?? false)
							break;
					}

					var ageGroup = new AgeGroupConfig() {
						Id = oldAgeGroup.Id,
						Name = oldAgeGroup.Name,
						Groups = groups
					};

					ageGroups.Add(ageGroup);

					if (token?.IsCancellationRequested ?? false)
						break;
				}

				return ageGroups;
			});
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
				SchoolWeeks = [],
				Teacher = TeacherConfig.Default,
				Blocks = []
			};
		}

		public GroupConfigurationBuilder SetName(string name) {
			CreateCurrentIfNull();
			current!.Name = name;
			return this;
		}

		public GroupConfigurationBuilder SetTeacher(TeacherConfig teacher) {
			CreateCurrentIfNull();
			current!.Teacher = teacher;
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
