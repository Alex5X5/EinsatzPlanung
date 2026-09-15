namespace Einsatzplanung.Input.Services;

using Einsatzplanung.Excel.Models;
using Einsatzplanung.Excel.Services;
using Einsatzplanung.Types.Models;

using Einsatzplanung.Input.Interfaces;

using System;
using System.Collections.Generic;

public class TeacherService : IEntityService<Teacher> {

	private const int NAME_COLUMN_INDEX = 0;
	private const int ABBREVIATION_COLUMN_INDEX = 1;
	private const int SPECIALIZATION_COLUMN_INDEX = 2;
	private const int WEEKLY_HOURS_COLUMN_INDEX = 3;

	private ExcelImportService excelImportService;

	private string SourceFilePath { get; set; } = "";

	public TeacherService(ExcelImportService excelImportService) {
		this.excelImportService = excelImportService;
	}

	public void SetSource(string path) {
		SourceFilePath = path;
	}

	public List<Teacher> ParseSource() {
		Table table = excelImportService.GetTable(SourceFilePath);
		List<Teacher> teachers = [];
		Teacher? teacher = null;
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
