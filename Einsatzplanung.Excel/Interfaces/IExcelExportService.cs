namespace Einsatzplanung.Excel.Interfaces;

public interface IExcelImportService {

	Models.Table GetTable(string path, int tableIndex = 1);

}
