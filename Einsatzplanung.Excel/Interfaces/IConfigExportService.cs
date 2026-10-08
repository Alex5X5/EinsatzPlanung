namespace Einsatzplanung.Excel.Interfaces;

using System.Collections.Generic;

public interface IConfigExportService<T> {
	void Export(string filePath, List<T> entities);
}