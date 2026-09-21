namespace Einsatzplanung.Excel.Services;

using ClosedXML.Excel;

public class ExcelExportService {

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
			}
			rowIndex++;
		}

		workbook.SaveAs(path);
	}
}
