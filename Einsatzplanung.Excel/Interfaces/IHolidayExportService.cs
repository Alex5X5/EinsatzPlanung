namespace Einsatzplanung.Excel.Interfaces;

using Einsatzplanung.Types.Models.Configuration;
using System.Collections.Generic;

public interface IHolidayExportService {
	void ExportHolidays(string filePath, List<HolidayConfig> holidays);
}
