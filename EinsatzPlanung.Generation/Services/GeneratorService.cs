namespace Einsatzplanung.Generation.Services;

using ClosedXML.Excel;

using Einsatzplanung.Generation.Interfaces;
using Einsatzplanung.Types.Models;
using Einsatzplanung.Util.Services;

using Einsatzplanung.Input.Interfaces;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Einsatzplanung.Types.Models.Generation;

public class GeneratorService : IGeneratorService {

	private static HashSet<DateTime> PublicHolidays { set; get; } = new() {
		new DateTime(2026, 10, 3),
		new DateTime(2026, 10, 31),
		new DateTime(2026, 11, 18),
		new DateTime(2026, 12, 25),
		new DateTime(2026, 12, 26),
		new DateTime(2027, 1, 1),
		new DateTime(2027, 3, 26),
		new DateTime(2027, 3, 29),
		new DateTime(2027, 5, 1),
		new DateTime(2027, 5, 6),
		new DateTime(2027, 5, 17)
	};

	private static readonly Dictionary<string, XLColor> SpecialColors = new() {
		["X"] = XLColor.FromHtml("#D9EAD3"),
		["U"] = XLColor.FromHtml("#E7E6E6"),
		["F"] = XLColor.FromHtml("#FCE5CD"),
		["!"] = XLColor.FromHtml("#FF0000")
	};

	private List<Teacher> teachers;
	private List<AgeGroup> ageGroups;
	private List<Group> groups;
	private List<DateTime> weekStarts;

	public GeneratorService(IEntityService<AgeGroup> ageGroupService, IEntityService<Teacher> teacherService) {
		teachers = teacherService.ParseSource();
		ageGroups = ageGroupService.ParseSource();
		groups = ageGroups.SelectMany(ageGroup => ageGroup.Groups).ToList();
		weekStarts = GetWeekStarts(new DateTime(2026, 8, 17), new DateTime(2027, 7, 31));
	}

	public Plan GeneratePlan() {
		//const string outputPath = "Einsatzplan_2026_2027.xlsx";

		return BuildPlan();
		//ExportPlan(outputPath, assignments);
	}

	private static List<DateTime> GetWeekStarts(DateTime schoolYearStart, DateTime schoolYearEnd) {
		var firstMonday = DateTimeService.FloorWeek(schoolYearStart);

		var weekStarts = new List<DateTime>();
		for (var current = firstMonday; current <= schoolYearEnd.Date; current = current.AddDays(7)) {
			weekStarts.Add(current);
		}

		return weekStarts;
	}

	private static bool IsSchoolWeek(Group group, DateTime date) {
		return group.SchoolWeeks.Contains(WeekOfYear(date));
	}

	private static bool IsVacation(Group group, DateTime date) {
		return false;
		//return group.VacationPeriods.Any(period =>
		//	date.Date >= period.Von.Date && date.Date <= period.Bis.Date);
	}

	private static bool IsVacationWeek(Group group, DateTime weekStart) {
		return false;
		//var weekEnd = weekStart.AddDays(4);
		//return group.VacationPeriods.Any(period =>
		//	period.Von.Date <= weekEnd.Date && period.Bis.Date >= weekStart.Date);
	}

	private static bool WeekHasOperationalDay(Group group, DateTime weekStart) {
		return false;
		//for (var dayOffset = 0; dayOffset < 5; dayOffset++) {
		//	var day = weekStart.AddDays(dayOffset);

		//	if (PublicHolidays.Contains(day.Date) || IsSchoolWeek(group, day) || IsVacation(group, day)) {
		//		continue;
		//	}

		//	return true;
		//}

		//return false;
	}

	private static Block? GetBlockForWeek(Group group, DateTime weekStart) {
		return null;
		//var weekEnd = weekStart.AddDays(6);
		//return group.Blocks.FirstOrDefault(block =>
		//	block.From.Date <= weekEnd.Date && block.To.Date >= weekStart.Date);
	}

	private static int WeekOfYear(DateTime date) {
		var day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(date);
		if (day is >= DayOfWeek.Monday and <= DayOfWeek.Wednesday) {
			date = date.AddDays(3);
		}

		return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(
			date,
			CalendarWeekRule.FirstFourDayWeek,
			DayOfWeek.Monday);
	}

	private Plan BuildPlan() {

		Plan plan = new();

		// Used only to distribute equivalent qualified trainers fairly over the whole school year.
		Dictionary<Teacher, int> assignmentsPerTeacher = teachers.ToDictionary(teacher => teacher, teacher => 0);

		//loop through every week in the 
		foreach (var weekStart in weekStarts) {
			List<PlanItem> planItems = [];

			foreach (var group in groups) {
				Block? block = GetBlockForWeek(group, weekStart);
				if (block is null)
					continue;
				if (IsSchoolWeek(group, weekStart))
					continue;
				if (IsVacationWeek(group, weekStart))
					continue;
				if (!WeekHasOperationalDay(group, weekStart))
					continue;
				int qualifiedCount = teachers.Count(teacher => teacher.CanTeachBlock(block));
				planItems.Add(new PlanItem() { Block = block, Group = group, QualifiedCount = qualifiedCount });
			}

			// Classes with fewer qualified trainers are planned first.
			// This avoids using a specialist for a flexible class before a specialist-only class is assigned.
			planItems = planItems
				.OrderBy(item => item.QualifiedCount)
				.ThenBy(item => item.Group.Name)
				.ToList();
			
			//keep track, which teachers are already assigned this week
			var occupiedTrainers = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			
			foreach (var item in planItems) {
				// All trainers with the matching topic are equal candidates.
				// Least total prior assignments wins, then alphabetically by abbreviation for deterministic output.
				var selectedTrainer = teachers
					.Where(trainer => trainer.CanTeachBlock(item.Block))
					.Where(trainer => !occupiedTrainers.Contains(trainer.Abbreviation))
					.OrderBy(trainer => assignmentsPerTeacher[trainer])
					.ThenBy(trainer => trainer.Abbreviation, StringComparer.OrdinalIgnoreCase)
					.FirstOrDefault();

				if (selectedTrainer is null) {
					Console.WriteLine(
						$"WARNUNG: KW {WeekOfYear(weekStart)}: Kein qualifizierter und freier Ausbilder fuer " +
						$"{item.Group.Name}, Thema: {item.Block.Name}.");
					continue;
				}

				occupiedTrainers.Add(selectedTrainer.Abbreviation);
				assignmentsPerTeacher[selectedTrainer]++;

				plan.Assignments[item.Group][weekStart] = new Assignment() {
					Trainer = selectedTrainer.Abbreviation,
					BlockName = item.Block.Name,
					BlockColor = item.Block.Farbe
				};
			}
		}
		return plan;
	}

	private (string Value, string? Color) DetermineCellValue(Group group, DateTime date, Assignment? assignment) {
		// Priority: legal holiday -> school week -> apprentice vacation -> assignment.
		if (PublicHolidays.Contains(date.Date)) {
			return ("F", null);
		}

		if (IsSchoolWeek(group, date)) {
			return ("X", null);
		}

		if (IsVacation(group, date)) {
			return ("U", null);
		}

		if (assignment is null) {
			return ("!", "#FF0000");
		}

		return (assignment.Trainer, assignment.BlockColor);
	}

	private void ExportPlan(string outputPath, Plan plan) {
		using var workbook = new XLWorkbook();
		var worksheet = workbook.Worksheets.Add("2026_2027");

		const int titleRow = 1;
		const int headerDateRow = 3;
		const int headerWeekRow = 4;
		const int firstWeekColumn = 6;
		var lastWeekColumn = firstWeekColumn + weekStarts.Count - 1;

		worksheet.Range(titleRow, 1, titleRow, lastWeekColumn).Merge();
		worksheet.Cell(titleRow, 1).Value =
			$"Fachinformatiker | Einsatzplan der Ausbilder / Ausbildungsinhalte ";// +
			//$"{config.SchuljahrStart:yyyy}/{config.SchuljahrEnde:yyyy}";
		worksheet.Cell(titleRow, 1).Style.Font.Bold = true;
		worksheet.Cell(titleRow, 1).Style.Font.FontSize = 14;
		worksheet.Cell(titleRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
		worksheet.Cell(titleRow, 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#1F4E78");
		worksheet.Cell(titleRow, 1).Style.Font.FontColor = XLColor.White;

		worksheet.Cell(headerDateRow, 1).Value = "Lehrjahr";
		worksheet.Cell(headerDateRow, 2).Value = "Untergruppe";
		worksheet.Cell(headerDateRow, 3).Value = "Gruppen-Ausbilder";
		worksheet.Cell(headerDateRow, 4).Value = "Tag";
		worksheet.Cell(headerDateRow, 5).Value = "KW";

		for (var i = 0; i < weekStarts.Count; i++) {
			var column = firstWeekColumn + i;
			worksheet.Cell(headerDateRow, column).Value = weekStarts[i].ToString("dd.MM.yyyy");
			worksheet.Cell(headerWeekRow, column).Value = WeekOfYear(weekStarts[i]);
		}

		var headerRange = worksheet.Range(headerDateRow, 1, headerWeekRow, lastWeekColumn);
		headerRange.Style.Font.Bold = true;
		headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#D9EAF7");
		headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
		headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
		headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
		headerRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

		var row = headerWeekRow + 1;
		foreach (var group in groups) {
			worksheet.Range(row, 1, row + 4, 1).Merge();
			worksheet.Cell(row, 1).Value = "group.YearLabel";

			worksheet.Range(row, 2, row + 4, 2).Merge();
			worksheet.Cell(row, 2).Value = group.Name;

			worksheet.Range(row, 3, row + 4, 3).Merge();
			worksheet.Cell(row, 3).Value = "group.DisplayTrainer" ?? "-";

			foreach (var cell in new[]
					 {
						 worksheet.Cell(row, 1),
						 worksheet.Cell(row, 2),
						 worksheet.Cell(row, 3)
					 }) {
				cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
				cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
				cell.Style.Font.Bold = true;
				cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
			}

			for (var dayIndex = 0; dayIndex < 5; dayIndex++) {
				var currentRow = row + dayIndex;
				worksheet.Cell(currentRow, 4).Value = new[] { "Mo", "Di", "Mi", "Do", "Fr" }[dayIndex];
				worksheet.Cell(currentRow, 4).Style.Font.Bold = true;
				worksheet.Cell(currentRow, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

				for (var weekIndex = 0; weekIndex < weekStarts.Count; weekIndex++) {
					DateTime weekStart = weekStarts[weekIndex];
					DateTime day = weekStart.AddDays(dayIndex);

					Assignment? assignment = plan.Assignments[group][weekStart];
					
					var (value, color) = DetermineCellValue(group, day, assignment);
					var cell = worksheet.Cell(currentRow, firstWeekColumn + weekIndex);
					cell.Value = value;
					ApplyCellStyle(cell, value, color);
				}
			}

			row += 5;
		}

		WriteLegend(worksheet, Math.Max(row + 2, 45));

		worksheet.Column(1).Width = 10;
		worksheet.Column(2).Width = 14;
		worksheet.Column(3).Width = 17;
		worksheet.Column(4).Width = 6;
		worksheet.Column(5).Width = 8;
		for (var column = firstWeekColumn; column <= lastWeekColumn; column++) {
			worksheet.Column(column).Width = 5;
		}

		worksheet.SheetView.FreezeRows(headerWeekRow);
		worksheet.SheetView.FreezeColumns(5);
		worksheet.PageSetup.PageOrientation = XLPageOrientation.Landscape;
		worksheet.PageSetup.FitToPages(1, 0);
		worksheet.PageSetup.Margins.Left = 0.15;
		worksheet.PageSetup.Margins.Right = 0.15;

		workbook.SaveAs(outputPath);
	}

	private static void ApplyCellStyle(IXLCell cell, string value, string? blockColor) {
		cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
		cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
		cell.Style.Alignment.WrapText = true;
		cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
		cell.Style.Border.OutsideBorderColor = XLColor.FromHtml("#B7B7B7");

		if (!string.IsNullOrWhiteSpace(blockColor)) {
			cell.Style.Fill.BackgroundColor = XLColor.FromHtml(blockColor);
		} else if (SpecialColors.TryGetValue(value, out var specialColor)) {
			cell.Style.Fill.BackgroundColor = specialColor;
		}

		if (value is "F" or "U" or "X" or "!") {
			cell.Style.Font.Bold = true;
		}
	}

	private void WriteLegend(IXLWorksheet worksheet, int row) {
		worksheet.Cell(row, 1).Value = "Legende:";
		worksheet.Cell(row, 1).Style.Font.Bold = true;

		worksheet.Cell(row + 1, 1).Value = "X";
		worksheet.Cell(row + 1, 2).Value = "Berufsschulwoche (kein Ausbilder eingeplant)";
		worksheet.Cell(row + 2, 1).Value = "U";
		worksheet.Cell(row + 2, 2).Value = "Urlaub der Azubis (kein Ausbilder eingeplant)";
		worksheet.Cell(row + 3, 1).Value = "F";
		worksheet.Cell(row + 3, 2).Value = "Gesetzlicher Feiertag Sachsen";
		worksheet.Cell(row + 4, 1).Value = "!";
		worksheet.Cell(row + 4, 2).Value = "Kein qualifizierter Ausbilder frei";

		var trainerRow = row + 6;
		worksheet.Cell(trainerRow, 1).Value = "Ausbilder:";
		worksheet.Cell(trainerRow, 1).Style.Font.Bold = true;

		for (var i = 0; i < teachers.Count; i++) {
			var trainer  = teachers[i];
			worksheet.Cell(trainerRow + 1 + i, 1).Value = trainer.Abbreviation;
			worksheet.Cell(trainerRow + 1 + i, 2).Value = trainer.Name;
			worksheet.Cell(trainerRow + 1 + i, 3).Value = $"{trainer.WeeklyHours}h/Woche";
			worksheet.Cell(trainerRow + 1 + i, 4).Value = string.Join(", ", trainer.Specializations);
		}
	}
}
