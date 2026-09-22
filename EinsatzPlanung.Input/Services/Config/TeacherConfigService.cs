namespace Einsatzplanung.Input.Services.Config;

using Einsatzplanung.Excel.Models;
using Einsatzplanung.Excel.Services;
using Einsatzplanung.Types.Models;

using Einsatzplanung.Input.Interfaces;

using System;
using System.Collections.Generic;
using Einsatzplanung.Types.Models.Configuration;

public class TeacherConfigService : IConfigService<TeacherConfig> {

	private const int NAME_COLUMN_INDEX = 0;
	private const int ABBREVIATION_COLUMN_INDEX = 1;
	private const int SPECIALIZATION_COLUMN_INDEX = 2;
	private const int WEEKLY_HOURS_COLUMN_INDEX = 3;

	private ExcelImportService excelImportService;

	private string SourceFilePath { get; set; } = "";

	public TeacherConfigService(ExcelImportService excelImportService) {
		this.excelImportService = excelImportService;
	}

	public void SetSource(string path) {
		SourceFilePath = path;
	}

	public List<TeacherConfig> ParseSource() {
		Table table = excelImportService.GetTable(SourceFilePath);

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
			if (!string.IsNullOrEmpty(table[row, SPECIALIZATION_COLUMN_INDEX]?.Value))
				builder.AddSpecialization(table[row, SPECIALIZATION_COLUMN_INDEX]!.Value);
		}

		if (builder.Build() is TeacherConfig t)
			teachers.Add(t);

		return teachers;
	}

	private class TeacherConfigBuilder {

		private TeacherConfig? current;

		private void CreateCurrentIfNull() {
			current ??= new TeacherConfig() {
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

		public TeacherConfigBuilder AddSpecialization(string specialization) {
			CreateCurrentIfNull();
			current!.Specializations.Add(new Topic(specialization));
			return this;
		}

		public TeacherConfig? Build() {
			return current;
		}
	}
}
