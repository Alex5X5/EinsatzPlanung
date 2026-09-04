namespace Einsatzplanung.Excel.Services;

using ClosedXML.Excel;

using System;

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

		XLWorkbook workbook = new(path);
		var worksheet = workbook.Worksheet(table.Index);

		int rowIndex = 0;
		foreach (var row in table.Cells) {
			int colIndex = 0;
			foreach(var cell in row) {
				var worksheetCell = worksheet.Cell(GetCellAddress(rowIndex, colIndex));
				worksheetCell.Value = cell.Value;
				worksheetCell.Style.Fill.BackgroundColor = XLColor.FromArgb(cell.BackgroundColor.A, cell.BackgroundColor.R, cell.BackgroundColor.G, cell.BackgroundColor.B);
				colIndex++;
			}
			rowIndex++;
		}
	}
}
