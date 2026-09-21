namespace Einsatzplanung.Excel.Services;

using ClosedXML.Excel;
using System.Collections.Generic;

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

	public void SaveTableToFile(string path, Models.Table table) {

		using var workbook = new XLWorkbook();
		var worksheet = workbook.Worksheets.Add("Export");

		int rowIndex = 0;
		foreach (var row in table.Cells) {
			int colIndex = 0;
			foreach(var cell in row) {
				var worksheetCell = worksheet.Cell(rowIndex + 1, colIndex + 1);
				worksheetCell.Value = cell.Value;
				colIndex++;
	public void SaveTable(string path, Models.Table table, int tableIndex = 1) {
		XLWorkbook workbook = new(path);
		var worksheet = workbook.Worksheet(tableIndex);
		for (int row = 0; row < table.RowCount; row++) {
			for (int col = 0; col < table.ColumnCount; col++) {
				var cell = worksheet.Cell(row, col);
				cell.Value = table[row, col]?.Value ?? "";
				cell.Value = table[row, col]?.BackgroundColor.ToString() ?? "#FFFFFFFF";
			}
		}

		workbook.SaveAs(path);
	}
}
