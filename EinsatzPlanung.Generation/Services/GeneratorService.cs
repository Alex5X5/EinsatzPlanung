namespace Einsatzplanung.Generation.Services;

using Einsatzplanung.Generation.Interfaces;
using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models;
using Einsatzplanung.Types.Models.Generation;
using Einsatzplanung.Util.Services;

using System;
using System.Collections.Generic;
using System.Linq;

public class GeneratorService : IGeneratorService {

	private readonly List<Teacher> teachers;
	private readonly List<Group> groups;
	private readonly List<DateTime> weekStarts;

	public GeneratorService(IEntityService<AgeGroup> ageGroupService, IEntityService<Group> groupService, IEntityService<Teacher> teacherService, GeneralConfigService configService) {
		groups = groupService.GetEntities();
		teachers = teacherService.GetEntities();
		weekStarts = DateTimeService.GetWeekStartsInIntervall(configService.YearStartDate, configService.YearEndDate);
	}

	private static bool IsSchoolWeek(Group group, DateTime date) {
		return group.SchoolWeeks.Contains(DateTimeService.WeekOfYear(date));
	}

	private static bool IsVacationWeek(Group group, DateTime weekStart) {
		var weekEnd = weekStart.AddDays(5);
		return group.Holidays.Any(
			period =>
				period.From <= DateOnly.FromDateTime(weekEnd) &&
				period.To >= DateOnly.FromDateTime(weekEnd));
	}

	private Teacher? FindAvailableTeacher(Group group, Block block, DateTime week, Dictionary<string, HashSet<DateTime>> trainerBusyWeeks) {
		// Prefer the group's own trainer when they're qualified and free that week.
		if (group.Teacher.CanTeachBlock(block) && !trainerBusyWeeks[group.Teacher.Abbreviation].Contains(week)) {
			return group.Teacher;
		}

		// Otherwise fall back to any other qualified, free trainer.
		return teachers.FirstOrDefault(t =>
			t.CanTeachBlock(block) &&
			!trainerBusyWeeks[t.Abbreviation].Contains(week));
	}

	public Plan GeneratePlan() {
		var plan = new Plan();

		// Tracks which weeks each trainer is already committed to, across all groups.
		var trainerBusyWeeks = teachers.ToDictionary(t => t.Abbreviation, t => new HashSet<DateTime>());

		// loop through each group
		foreach (var group in groups) {
			
			//save the assignments of the current group in a dictionary
			//the key is the week's date and the value is the assignment
			Dictionary<DateTime, Assignment> groupAssignments = [];
			
			plan.Assignments[group] = groupAssignments;

			// only assign blocks to weeks, that aren't school weeks or vacations
			List<DateTime> validWeeks = weekStarts
				.Where(week => !IsSchoolWeek(group, week))
				.Where(week => !IsVacationWeek(group, week))
				.OrderBy(week => week)
				.ToList();
			
			//start at week 0
			var weekIndex = 0;

			foreach (Block block in group.Blocks) {

				//loop through each week of the current block and try to assign it
				for (int assigned = 0; assigned < block.Count; assigned++) {
					
					if (weekIndex >= validWeeks.Count)
						throw new InvalidOperationException(
							$"Not enough remaining valid weeks to schedule block '{block.Name}' for group '{group.Name}'.");
					
					DateTime week = validWeeks[weekIndex++];

					Teacher? trainer = FindAvailableTeacher(group, block, week, trainerBusyWeeks);
					if (trainer is null)
						throw new InvalidOperationException(
							$"No qualified, available trainer found for block '{block.Name}' " +
							$"(group '{group.Name}', week {DateTimeService.WeekOfYear(week)}).");

					trainerBusyWeeks[trainer.Abbreviation].Add(week);

					groupAssignments[week] = new Assignment {
						Trainer = trainer.Abbreviation,
						BlockName = block.Name,
						BlockColor = block.Color
					};
				}
			}
		}

		return plan;
	}
}
