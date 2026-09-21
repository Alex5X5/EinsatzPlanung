namespace Einsatzplanung.Excel.Services;

using ClosedXML.Excel;
using Einsatzplanung.Excel.Interfaces;
using Einsatzplanung.Util.Services;
using Einsatzplanung.Types.Models;
using Einsatzplanung.Types.Models.Generation;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;

public sealed class PlanExportService : IPlanExportService
{
	private readonly DateTime _yearStartDate;
	private readonly DateTime _yearEndDate;

	public PlanExportService(GeneralConfigService configService) {
		_yearStartDate = configService.YearStartDate;
		_yearEndDate = configService.YearEndDate;
	}

	public void ExportPlan(string filePath, Plan plan)
	{
		XLWorkbook workbook = new();
		var worksheet = workbook.Worksheets.Add($"{_yearStartDate.Year}_{_yearStartDate.Year+1}");

		// Header
		worksheet.Cell("A1").Value = "Fachinformatiker";
		worksheet.Cell("B1").Value = $"Einsatzplan der Ausbilder / Ausbildungsinhalte {_yearStartDate.Year}/{_yearEndDate.Year}";

		// Column headers
		worksheet.Cell("A3").Value = "Lehrjahr";
		worksheet.Cell("B3").Value = "Untergruppe";
		worksheet.Cell("C3").Value = "Gruppen-Ausbilder";
		worksheet.Cell("D3").Value = "Tag";
		worksheet.Cell("E3").Value = "KW";

		// Calculate all weeks in range
		var weeks = GetWeeksInRange(_yearStartDate, _yearEndDate);

		// Week dates header
		int colIndex = 6; // Column F
		foreach (var weekStart in weeks) {
			worksheet.Cell(3, colIndex).Value = weekStart.ToString("dd.MM.yyyy");
			colIndex++;
		}

		// Week numbers header
		colIndex = 6;
		foreach (var weekStart in weeks)
		{
			worksheet.Cell(4, colIndex).Value = GetIsoWeek(weekStart);
			colIndex++;
		}

		// Data rows
		int rowIndex = 5;
		foreach (var (group, assignments) in plan.Assignments)
		{
			// 5 rows per group (Mo-Fr)
			for (int day = 0; day < 5; day++)
			{
				worksheet.Cell(rowIndex, 1).Value = day == 0 ? "1. Lj." : "";
				worksheet.Cell(rowIndex, 2).Value = day == 0 ? group : "";
				worksheet.Cell(rowIndex, 3).Value = day == 0 ? "" : "";
				worksheet.Cell(rowIndex, 4).Value = GetDayName(day);

				colIndex = 6;
				foreach (var weekStart in weeks)
				{
					DateTime currentDate = weekStart.AddDays(day);
					if (assignments.TryGetValue(currentDate, out var assignment))
					{
						worksheet.Cell(rowIndex, colIndex).Value = assignment.Trainer;

						if (ColorTranslator.FromHtml(assignment.BlockColor) is var color && color != Color.Empty)
						{
							worksheet.Cell(rowIndex, colIndex).Style.Fill.BackgroundColor = XLColor.FromColor(color);
						}
					}
					else
					{
						worksheet.Cell(rowIndex, colIndex).Value = "X";
					}
					colIndex++;
				}

				rowIndex++;
			}
		}

		worksheet.Columns().AdjustToContents();
		workbook.SaveAs(filePath);
	}

	public void ExportWeeklyPlan(string filePath, Dictionary<(string Group, DateTime WeekStart), Assignment> weeklyAssignments)
	{
		XLWorkbook workbook = new();
		var worksheet = workbook.Worksheets.Add("2026_2027");

		// Header
		worksheet.Cell("A1").Value = "Fachinformatiker";
		worksheet.Cell("B1").Value = $"Einsatzplan der Ausbilder / Ausbildungsinhalte {_yearStartDate.Year}/{_yearEndDate.Year}";

		// Column headers
		worksheet.Cell("A3").Value = "Lehrjahr";
		worksheet.Cell("B3").Value = "Untergruppe";
		worksheet.Cell("C3").Value = "Gruppen-Ausbilder";
		worksheet.Cell("D3").Value = "Tag";
		worksheet.Cell("E3").Value = "KW";

		// Calculate all weeks in range
		var weeks = GetWeeksInRange(_yearStartDate, _yearEndDate);

		// Week dates header
		int colIndex = 6; // Column F
		foreach (var weekStart in weeks)
		{
			worksheet.Cell(3, colIndex).Value = weekStart.ToString("dd.MM.yyyy");
			colIndex++;
		}

		// Week numbers header
		colIndex = 6;
		foreach (var weekStart in weeks)
		{
			worksheet.Cell(4, colIndex).Value = GetIsoWeek(weekStart);
			colIndex++;
		}

		// Get unique groups
		var uniqueGroups = weeklyAssignments.Select(kvp => kvp.Key.Group).Distinct().OrderBy(g => g).ToList();

		// Data rows
		int rowIndex = 5;
		foreach (var groupName in uniqueGroups)
		{
			// 5 rows per group (Mo-Fr)
			for (int day = 0; day < 5; day++)
			{
				worksheet.Cell(rowIndex, 1).Value = day == 0 ? "1. Lj." : "";
				worksheet.Cell(rowIndex, 2).Value = day == 0 ? groupName : "";
				worksheet.Cell(rowIndex, 3).Value = day == 0 ? "" : "";
				worksheet.Cell(rowIndex, 4).Value = GetDayName(day);

				colIndex = 6;
				foreach (var weekStart in weeks)
				{
					if (weeklyAssignments.TryGetValue((groupName, weekStart), out var assignment))
					{
						worksheet.Cell(rowIndex, colIndex).Value = assignment.Trainer;

						if (ColorTranslator.FromHtml(assignment.BlockColor) is var color && color != Color.Empty)
						{
							worksheet.Cell(rowIndex, colIndex).Style.Fill.BackgroundColor = XLColor.FromColor(color);
						}
					}
					else
					{
						worksheet.Cell(rowIndex, colIndex).Value = "X";
					}
					colIndex++;
				}

				rowIndex++;
			}
		}

		worksheet.Columns().AdjustToContents();
		workbook.SaveAs(filePath);
	}

	private static List<DateTime> GetWeeksInRange(DateTime startDate, DateTime endDate) {
		var weeks = new List<DateTime>();
		DateTime currentWeek = DateTimeService.FloorWeek(startDate);

		while (currentWeek <= endDate)
		{
			weeks.Add(currentWeek);
			currentWeek = currentWeek.AddDays(7);
		}

		return weeks;
	}

	private static int GetIsoWeek(DateTime date)
	{
		DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(date);
		if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
		{
			date = date.AddDays(3);
		}

		return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
	}

	private static string GetDayName(int dayIndex)
	{
		return dayIndex switch
		{
			0 => "Mo",
			1 => "Di",
			2 => "Mi",
			3 => "Do",
			4 => "Fr",
			_ => ""
		};
	}
}
