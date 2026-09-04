namespace Einsatzplanung.Excel.Services;

using System;
using System.Collections.Generic;
//using System.Collections.Generic;
using System.IO;
using System.Linq;
//using System.Text;

using ClosedXML.Excel;

using Einsatzplanung.Util.Services;

public class ExcelImportService {

	
	private System.Drawing.Color GetColorFromTheme(IXLTheme theme, XLThemeColor color) {
		return color switch {
			XLThemeColor.Background1 => theme.Background1.Color,
			XLThemeColor.Background2 => theme.Background2.Color,
			XLThemeColor.Accent1 => theme.Accent1.Color,
			XLThemeColor.Accent2 => theme.Accent2.Color,
			XLThemeColor.Accent3 => theme.Accent3.Color,
			XLThemeColor.Accent4 => theme.Accent4.Color,
			XLThemeColor.Accent5 => theme.Accent5.Color,
			XLThemeColor.Accent6 => theme.Accent6.Color,
			XLThemeColor.Text1 => theme.Text1.Color,
			XLThemeColor.Text2 => theme.Text2.Color,
			XLThemeColor.Hyperlink => theme.Hyperlink.Color,
			XLThemeColor.FollowedHyperlink => theme.FollowedHyperlink.Color,
			_ => throw new InvalidOperationException("Can not resolve theme color")
		};
	}

	private List<List<Models.TableCell>> GetCellsFromFile(string path, int tableIndex) {
		List<List<Models.TableCell>> cells = [];
		if (!File.Exists(path))
			return [];
		XLWorkbook workbook = new(path);
		var worksheet = workbook.Worksheet(tableIndex);

		foreach (var row in worksheet.RowsUsed()) {
			List<Models.TableCell> rowCells = [];
			foreach (var cell in row.CellsUsed()) {
				System.Drawing.Color background;
				if (cell.Style.Fill.BackgroundColor.ColorType == XLColorType.Theme) {
					background = GetColorFromTheme(workbook.Theme, cell.Style.Fill.BackgroundColor.ThemeColor);
				} else {
					background = cell.Style.Fill.BackgroundColor.Color;
				}
				rowCells.Add(new Models.TableCell() {
					Value = cell.Value.ToString(),
					BackgroundColor = new Avalonia.Media.Color(background.A, background.R, background.G, background.B)
				});
			}
			cells.Add(rowCells);
		}

		return cells;
	}

	public Models.Table GetTable(string path, int tableIndex) {
		List<List<Models.TableCell>> cells = GetCellsFromFile(path, tableIndex);

		return new Models.Table() {
			Cells = cells
		};
	}
	
	public Models.Table CreateTableObj(string path, int tableIndex = 1)
	{
		string filePath = PathService.AssetsPath(path);

		Models.Table table = new() 
		{
			Cells = new List<List<Models.TableCell>>()
		};
		
		if (!File.Exists(filePath))
		{
			Console.WriteLine($"File not found: {filePath}");
			return table;
		}

		XLWorkbook workbook = new(filePath);
		var worksheet = workbook.Worksheet(tableIndex);

		try 
		{
			foreach (var row in worksheet.RowsUsed())
			{
				var tableRow = new List<Models.TableCell>();

				foreach (var cell in row.CellsUsed())
				{
					if (cell != null)
					{
						tableRow.Add(new Models.TableCell
						{
							Value = cell.Value.ToString(),
							Bold = cell.Style.Font.Bold,
							TextRotation = cell.Style.Alignment.TextRotation,
							BackgroundColor1 = cell.Style.Fill.BackgroundColor.ToString()
						});
					}
				}
				table.Cells.Add(tableRow);
			}
			Console.WriteLine("Loaded Table ;)");
		} 
		catch (Exception e) 
		{
			Console.WriteLine($"Exception occured: {e}");
		}

		return table;
	}

}
