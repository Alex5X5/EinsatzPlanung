namespace Einsatzplanung.Excel.Services;

using Einsatzplanung.Excel.Interfaces;
using Einsatzplanung.Excel.Models;
using Einsatzplanung.Types.Models.Configuration;
using System.Collections.Generic;

public class TeacherExportService : IConfigExportService<TeacherConfig> {
	
	private readonly IExcelExportService excelExportService;

	public TeacherExportService(IExcelExportService excelExportService) {
		this.excelExportService = excelExportService;
	}

	public void Export(string filePath, List<TeacherConfig> teachers) {
		Table table = new();
		table.AddRow(["Name", "Kürzel", "Themen", "Wochenstunden"]);

		foreach (TeacherConfig teacher in teachers) {
			int rowCount = System.Math.Max(1, teacher.Specializations.Count);
			for (int index = 0; index < rowCount; index++) {
				bool firstRow = index == 0;
				table.AddRow([
					firstRow ? teacher.Name : "",
					firstRow ? teacher.Abbreviation : "",
					index < teacher.Specializations.Count ? teacher.Specializations[index].Name : "",
					firstRow ? teacher.WeeklyHours.ToString(System.Globalization.CultureInfo.InvariantCulture) : ""
				]);
			}
		}

		excelExportService.SaveTable(filePath, table, sheetName: "Ausbilder");
	}
}
