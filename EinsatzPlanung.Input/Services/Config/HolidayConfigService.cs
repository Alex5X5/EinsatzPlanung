using System;

namespace Einsatzplanung.Input.Services.Config;

using Einsatzplanung.Excel.Models;
using Einsatzplanung.Excel.Services;
using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models.Configuration;

using System.Collections.Generic;
using System.Globalization;

public class HolidayConfigService : IConfigService<HolidayConfig> {
    
    private const int FROM_COLUMN_INDEX = 0;
    private const int TO_COLUMN_INDEX = 1;

    private ExcelImportService excelImportService;
    private string SourceFilePath { get; set; } = "";

    public HolidayConfigService(ExcelImportService excelImportService) {
        this.excelImportService = excelImportService;
    }

    public void SetSource(string path) {
        SourceFilePath = path;
    }

    public List<HolidayConfig> ParseSource() {
        Table table = excelImportService.GetTable(SourceFilePath);
        List<HolidayConfig> holidays = [];

        for (int row = 1; row < table.RowCount; row++) {
            if (DateTime.TryParseExact( table[row, FROM_COLUMN_INDEX]?.Value,
				"dd.MM.yyyy HH:mm:ss",
				CultureInfo.InvariantCulture,
				DateTimeStyles.None, 
				out var from)) {

				HolidayConfig config = new() {
					From = DateOnly.FromDateTime(from)
				};
				
                if (DateOnly.TryParse(table[row, TO_COLUMN_INDEX]?.Value, out var to))
                    config.To = to;

                holidays.Add(config);
            }
        }

        return holidays;
    }
}
