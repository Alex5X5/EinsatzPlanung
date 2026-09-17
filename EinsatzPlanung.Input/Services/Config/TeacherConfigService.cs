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
		TeacherConfig? teacher = null;
		for (int row = 1; row < table.RowCount; row++) {
			if (table[row, ABBREVIATION_COLUMN_INDEX]?.Value != "") {
				if (teacher != null)
					teachers.Add(teacher);
				teacher = new() {
					Name = table[row, NAME_COLUMN_INDEX]?.Value ?? "",
					Abbreviation = table[row, ABBREVIATION_COLUMN_INDEX]?.Value ?? "",
					WeeklyHours = int.Parse(table[row, WEEKLY_HOURS_COLUMN_INDEX]?.Value ?? "0"),
					Specializations = []
				};
			}
			if(teacher == null)
				continue;
			teacher.Specializations.Add(new Topic(table[row, SPECIALIZATION_COLUMN_INDEX]?.Value ?? ""));
		}
		return teachers;
	}
}
