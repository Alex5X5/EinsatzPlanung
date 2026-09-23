namespace Einsatzplanung.Excel.Services;

using ClosedXML.Excel;
using System.Collections.Generic;
using System;
using System.IO;

public class ExcelExportService {
	
	private static readonly char[] ColumnNames = ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'];
	private static readonly int COLUMN_NAME_LENGTH = 26;

	private string GetCellAddress(int rowIndex, int colIndex) {
		string cellAddress = "";
		if (rowIndex >= COLUMN_NAME_LENGTH) {
			cellAddress = ColumnNames[(int)Math.Floor((double)rowIndex / COLUMN_NAME_LENGTH)].ToString();
			rowIndex -= COLUMN_NAME_LENGTH;
		}
		for (int i = 0; i < Math.Ceiling((double)colIndex / (double)COLUMN_NAME_LENGTH); i++)
			cellAddress += ColumnNames[colIndex];
		return cellAddress;
	}

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
