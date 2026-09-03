namespace Einsatzplanung.Excel.Services;

using System;
//using System.Collections.Generic;
using System.IO;
//using System.Text;

using ClosedXML.Excel;
// using DocumentFormat.OpenXml.Spreadsheet;
using Einsatzplanung.Util.Services;

public class ExcelImportService {
    
    public void PrintTable()
    {
        Console.WriteLine("AAAAAAAAAAAAAAAAAA");

        string filePath = PathService.AssetsPath("Excel\\ExcelEinsatzplanImput.xlsx");
        
        Console.WriteLine(filePath);

        if (!File.Exists(filePath))
        {
            Console.WriteLine($"File not found: {filePath}");
            return;
        } else
        {
            Console.WriteLine("File found!");
        }

        XLWorkbook workbook = new(filePath);
        var worksheet = workbook.Worksheet(1);

        foreach (var row in worksheet.RowsUsed())
        {
            foreach (var cell in row.CellsUsed())
            {
                Console.Write($"{cell.Value},");
            }
            Console.WriteLine();
        }


        
    }
   
}
