using System;

namespace Einsatzplanung.Input.Services.Config;

using Einsatzplanung.Excel.Models;
using Einsatzplanung.Excel.Services;
using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models.Configuration;

using System.Collections.Generic;

public class HolidayConfigService : IConfigService<HolidayConfig> {
    
    private const int VON_COLUMN_INDEX = 0;
    private const int BIS_COLUMN_INDEX = 1;

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
            string? vonValue = table[row, VON_COLUMN_INDEX]?.Value;
            string? bisValue = table[row, BIS_COLUMN_INDEX]?.Value;

            if (string.IsNullOrEmpty(vonValue))
                continue;

            if (DateOnly.TryParse(vonValue, out var von)) {
                DateOnly? bis = null;
                if (!string.IsNullOrEmpty(bisValue) && DateOnly.TryParse(bisValue, out var bisDate)) {
                    bis = bisDate;
                }

                holidays.Add(new HolidayConfig { Von = von, Bis = bis });
            }
        }

        return holidays;
    }
}
