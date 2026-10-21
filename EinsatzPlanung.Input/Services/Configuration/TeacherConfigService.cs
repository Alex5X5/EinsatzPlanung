namespace Einsatzplanung.Input.Services.Configuration;

using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

using Einsatzplanung.Excel.Models;
using Einsatzplanung.Excel.Interfaces;
using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models.Configuration;

public class TeacherConfigService : ConfigServiceBase<TeacherConfig> {

	private const int NAME_COLUMN_INDEX = 0;
	private const int ABBREVIATION_COLUMN_INDEX = 1;
	private const int SPECIALIZATION_COLUMN_INDEX = 2;
	private const int WEEKLY_HOURS_COLUMN_INDEX = 3;

	private IConfigService<TopicConfig> topicService;

	public TeacherConfigService(IExcelImportService importService, IConfigService<TopicConfig> topicService) : base(importService) {
		this.topicService = topicService;
	}

	protected override List<TeacherConfig> ParseTable(Table table) {
		List<TeacherConfig> teachers = [];
		
		TeacherConfigBuilder builder = new();
		
		for (int row = 1; row < table.RowCount; row++) {
			
			if (!string.IsNullOrEmpty(table[row, NAME_COLUMN_INDEX]?.Value)) {
				if (builder.Build() is TeacherConfig teacher)
					teachers.Add(teacher);
				builder = new TeacherConfigBuilder()
					.SetName(table[row, NAME_COLUMN_INDEX]?.Value ?? "")
					.SetAbbreviation(table[row, ABBREVIATION_COLUMN_INDEX]?.Value ?? "");
				if (int.TryParse(table[row, WEEKLY_HOURS_COLUMN_INDEX]?.Value ?? "", out var hours))
					builder.SetHours(hours);
			}

			string specialization = table[row, SPECIALIZATION_COLUMN_INDEX]?.Value ?? "";
			if (string.IsNullOrEmpty(specialization))
				continue;
			var topic = topicService.GetData().FirstOrDefault(t => t.Name == specialization);
			if(topic is null)
				continue;
			builder.AddSpecialization(topic);
		}

		if (builder.Build() is TeacherConfig t)
			teachers.Add(t);

		return teachers;
	}

	protected override async Task<List<TeacherConfig>> UpdateReferencesAsync(List<TeacherConfig> data) {
		return await Task.Run(
			() => {
				return data
					.Select(
						x =>
							new TeacherConfig() {
								Id = x.Id,
								Abbreviation = x.Abbreviation,
								Name = x.Name,
								Specializations = x.Specializations
									.Select(s => topicService.GetById(s.Id) ?? TopicConfig.Default)
									.ToList(),
								WeeklyHours = x.WeeklyHours
							})
					.ToList();
			});
	}

	private class TeacherConfigBuilder {

		private TeacherConfig? current;

		private void CreateCurrentIfNull() {
			current ??= new TeacherConfig() {
				Id = Guid.NewGuid(),
				Name = "",
				Abbreviation = "",
				WeeklyHours = 0,
				Specializations = []
			};
		}

		public TeacherConfigBuilder SetName(string name) {
			CreateCurrentIfNull();
			current!.Name = name;
			return this;
		}

		public TeacherConfigBuilder SetAbbreviation(string abbreviation) {
			CreateCurrentIfNull();
			current!.Abbreviation = abbreviation;
			return this;
		}

		public TeacherConfigBuilder SetHours(int hours) {
			CreateCurrentIfNull();
			current!.WeeklyHours = hours;
			return this;
		}

		public TeacherConfigBuilder AddSpecialization(TopicConfig specialization) {
			CreateCurrentIfNull();
			current!.Specializations.Add(specialization);
			return this;
		}

		public TeacherConfig? Build() {
			return current;
		}
	}
}
