namespace Einsatzplanung.Generation.Services;

using ClosedXML.Excel;

using Einsatzplanung.Generation.Interfaces;
using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models;
using Einsatzplanung.Types.Models.Generation;
using Einsatzplanung.Util.Services;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

public class GeneratorService : IGeneratorService {

	private readonly List<Teacher> teachers;
	private readonly List<Group> groups;
	private readonly List<DateTime> weekStarts;

	public GeneratorService(IEntityService<AgeGroup> ageGroupService, IEntityService<Group> groupService, IEntityService<Teacher> teacherService, GeneralConfigService configService) {
		groups = groupService.GetEntities();
		teachers = teacherService.GetEntities();
		weekStarts = GetWeekStarts(configService.YearStartDate, configService.YearEndDate);
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

	private static bool IsVacationWeek(Group group, DateTime weekStart) {
		var weekEnd = weekStart.AddDays(5);
		return group.Holidays.Any(
			period =>
				period.From <= DateOnly.FromDateTime(weekEnd) &&
				period.To >= DateOnly.FromDateTime(weekEnd));
	}

	private static Block? GetBlockForWeek(Group group, DateTime weekStart) {
		var weekEnd = weekStart.AddDays(5);
		return group.Blocks.FirstOrDefault(block => block.From.Date <= weekEnd.Date && block.To.Date >= weekStart.Date);
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

	public Plan GeneratePlan() {

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
				//if (!WeekHasOperationalDay(group, weekStart))
				//	continue;
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

				plan.Assignments.TryAdd(item.Group.Name, []);

				plan.Assignments[item.Group.Name][weekStart] = new Assignment() {
					Trainer = selectedTrainer.Abbreviation,
					BlockName = item.Block.Name,
					BlockColor = item.Block.Color
				};
			}
		}
		return plan;

	}
}
