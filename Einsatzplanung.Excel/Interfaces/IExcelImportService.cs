namespace Einsatzplanung.Excel.Interfaces;

public interface IExcelExportService {
	
	void SaveTable(string path, Models.Table table, int tableIndex = 1, string sheetName = "");

}
