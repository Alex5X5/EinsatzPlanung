namespace Einsatzplanung.Excel.Services;

using System;

using ClosedXML.Excel;

using Einsatzplanung.Excel.Interfaces;


public class ExcelExportService : IExcelExportService {

	public void SaveTable(string path, Models.Table table, int tableIndex = 1, string sheetName="") {
		XLWorkbook workbook = new();
		var worksheetName = string.IsNullOrEmpty(sheetName)
			? $"Tabelle {tableIndex}"
			: sheetName;
		var worksheet = workbook.Worksheets.Add(worksheetName);
		for (int row = 0; row < table.RowCount; row++) {
			for (int col = 0; col < table.ColumnCount; col++) {
				var cell = worksheet.Cell(row + 1, col + 1);
				cell.Value = table[row, col]?.Value ?? "";
			}
		}

		workbook.SaveAs(path);
	}
}
