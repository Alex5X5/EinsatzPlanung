namespace Einsatzplanung.Excel.Interfaces;

using System.Collections.Generic;
using System.Threading.Tasks;

public interface IConfigExportService<T> {
	Task ExportAsync(string filePath, List<T> entities);
}