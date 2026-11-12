namespace Einsatzplanung.Excel.Services;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;

using ClosedXML.Excel;

using Einsatzplanung.Excel.Interfaces;
using Einsatzplanung.Util.Services;
using Einsatzplanung.Types.Models.Generation;

public sealed class PlanExportService : IPlanExportService {
	
	private readonly DateTime _yearStartDate;
	private readonly DateTime _yearEndDate;

	private const int TITLE_ROW = 1;
	private const int HEADER_ROW = 3;

	private const int YEAR_COLUMN = 1;
	private const int GROUP_COLUMN = 2;
	private const int TEACHER_COLUMN = 3;
	private const int DAY_COLUMN = 4;
	private const int WEEK_COLUMN = 5;

	private const double YEAR_COLUMN_WIDTH = 16.0;
	private const double GROUP_COLUMN_WIDTH = 13.0;
	private const double TEACHER_COLUMN_WIDTH = 17.5;
	private const double DAY_COLUMN_WIDTH = 3.5;
	private const double WEEK_COLUMN_WIDTH = 3.5;

	public PlanExportService(GeneralConfigService configService) {
		_yearStartDate = configService.YearStartDate;
		_yearEndDate = configService.YearEndDate;
	}

	private void SetHeader(IXLWorksheet worksheet) {
		worksheet.Column(YEAR_COLUMN).Width = YEAR_COLUMN_WIDTH;
		worksheet.Column(GROUP_COLUMN).Width = GROUP_COLUMN_WIDTH;
		worksheet.Column(TEACHER_COLUMN).Width = TEACHER_COLUMN_WIDTH;
		worksheet.Column(DAY_COLUMN).Width = DAY_COLUMN_WIDTH;
		worksheet.Column(WEEK_COLUMN).Width = WEEK_COLUMN_WIDTH;

		worksheet.Range("B1:J1").Merge();

		// Header
		worksheet.Cell("A1").Value = "Fachinformatiker*in";
		worksheet.Cell("B1").Value = $"Einsatzplan der Ausbilder*innen / Ausbildungsinhalte {_yearStartDate.Year}/{_yearEndDate.Year}";

		// Column headers
		worksheet.Cell(HEADER_ROW, YEAR_COLUMN).Value = "Lehrjahr";
		worksheet.Cell(HEADER_ROW, GROUP_COLUMN).Value = "Untergruppe";
		worksheet.Cell(HEADER_ROW, TEACHER_COLUMN).Value = "Gruppen-Ausbilder*innen";
		worksheet.Cell(HEADER_ROW, DAY_COLUMN).Value = "Tag";
		worksheet.Cell(HEADER_ROW, WEEK_COLUMN).Value = "KW";
	}

	public void ExportPlan(string filePath, Plan plan) {
		XLWorkbook workbook = new();
		var worksheet = workbook.Worksheets.Add($"{_yearStartDate.Year}_{_yearStartDate.Year+1}");

		SetHeader(worksheet);

		// Calculate all weeks in range
		var weeks = GetWeeksInRange(_yearStartDate, _yearEndDate);

		// Week dates header
		int colIndex = 6; // Column F
		foreach (var weekStart in weeks) {
			var cell = worksheet.Cell(3, colIndex);
			cell.Value = weekStart.ToString("dd.MM.yyyy");
			cell.Style.Alignment.SetTextRotation(90);
			colIndex++;
		}

		// Week numbers header
		colIndex = 6;
		foreach (var weekStart in weeks)
		{
			worksheet.Cell(DAY_COLUMN, colIndex).Value = GetIsoWeek(weekStart);
			colIndex++;
		}

		// Data rows
		int row = 5;
		foreach (var (group, assignments) in plan.Assignments)
		{
			// 5 rows per group (Mo-Fr)
			for (int day = 0; day < 5; day++)
			{
				worksheet.Cell(row, YEAR_COLUMN).Value = day == 0 ? "1. Lj." : "";
				worksheet.Cell(row, GROUP_COLUMN).Value = day == 0 ? group.Name : "";
				worksheet.Cell(row, TEACHER_COLUMN).Value = day == 0 ? group.Teacher.Abbreviation : "";
				worksheet.Cell(row, DAY_COLUMN).Value = GetDayName(day);
				
				colIndex = 6;
				foreach (var weekStart in weeks) {
					// DateTime currentDate = weekStart.AddDays(day);
					if (assignments.TryGetValue(weekStart, out var assignment)) {
						if (assignment.IsHoliday[day]) {
							worksheet.Cell(row, colIndex).Value = "U";
							worksheet.Cell(row, colIndex).Style.Font.Bold = true;
							worksheet.Cell(row, colIndex).Style.Fill.BackgroundColor = XLColor.FromColor(Color.White);
						} else {
							worksheet.Cell(row, colIndex).Value = assignment.Trainer;
							if (ColorTranslator.FromHtml(assignment.BlockColor) is var color && color != Color.Empty)
								worksheet.Cell(row, colIndex).Style.Fill.BackgroundColor = XLColor.FromColor(color);
						}
					} else {
						worksheet.Cell(row, colIndex).Value = "X";
					}
					colIndex++;
				}

				row++;
			}
		}

		worksheet.Columns().AdjustToContents();
		workbook.SaveAs(filePath);
	}

	[Obsolete("This method is deprecated and will be removed in a future release. Please use ExportPlan instead.")]
	public void ExportWeeklyPlan(string filePath, Dictionary<(string Group, DateTime WeekStart), Assignment> weeklyAssignments)
	{
		XLWorkbook workbook = new();
		var worksheet = workbook.Worksheets.Add("2026_2027");

		// Header
		worksheet.Cell("A1").Value = "Fachinformatiker*in";
		worksheet.Cell("B1").Value = $"Einsatzplan der Ausbilder*innen / Ausbildungsinhalte {_yearStartDate.Year}/{_yearEndDate.Year}";

		// Column headers
		worksheet.Cell("A3").Value = "Lehrjahr";
		worksheet.Cell("B3").Value = "Untergruppe";
		worksheet.Cell("C3").Value = "Gruppen-Ausbilder*innen";
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
							Color foregroundColor = GetForeGroundColorFromBlockColor(color);
							worksheet.Cell(rowIndex, colIndex).Style.Font.FontColor = XLColor.FromColor(foregroundColor);
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

	private static Color GetForeGroundColorFromBlockColor(Color color) {
		double average = ((double)color.R)*0.9 + ((double)color.G) * 1.2 + ((double)color.B) * 0.9;
		
		average /= 3;
		
		if (average > 100) {
			return Color.FromArgb(255,0,0,0);
		} else {
			return Color.FromArgb(255,255,255,255);
		}
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
