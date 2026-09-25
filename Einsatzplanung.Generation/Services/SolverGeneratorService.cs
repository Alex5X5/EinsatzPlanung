namespace Einsatzplanung.Generation.Services;

using System;
using System.Collections.Generic;
using System.Linq;

using Google.OrTools.Sat;

using Einsatzplanung.Generation.Interfaces;
using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models;
using Einsatzplanung.Types.Models.Generation;
using Einsatzplanung.Util.Services;

public class SolverGeneratorService : IGeneratorService {

	private readonly List<Teacher> teachers;
	private readonly List<Group> groups;
	private readonly List<DateTime> weekStarts;

	public SolverGeneratorService(IEntityService<AgeGroup> ageGroupService, IEntityService<Group> groupService, IEntityService<Teacher> teacherService, GeneralConfigService configService) {
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

	public Plan GeneratePlan() {
		var plan = new Plan();

		var slots = new List<Slot>();

		foreach (var group in groups) {
			var validWeeks = weekStarts
				.Where(week => !IsSchoolWeek(group, week) && !IsVacationWeek(group, week))
				.OrderBy(week => week)
				.ToList();

			var weekIndex = 0;

			foreach (var block in group.Blocks) {
				for (var i = 0; i < block.Count; i++) {
					if (weekIndex >= validWeeks.Count) {
						throw new InvalidOperationException(
							$"Not enough remaining school weeks for block '{block.Name}' in group '{group.Name}'.");
					}

					slots.Add(new Slot {
						Group = group,
						Block = block,
						Week = validWeeks[weekIndex++]
					});
				}
			}
		}

		var model = new CpModel();

		var x = new Dictionary<(int slot, int trainer), BoolVar>();

		for (var s = 0; s < slots.Count; s++) {
			for (var t = 0; t < teachers.Count; t++) {
				if (teachers[t].CanTeachBlock(slots[s].Block)) {
					x[(s, t)] = model.NewBoolVar($"x_s{s}_t{t}");
				}
			}
		}

		for (var s = 0; s < slots.Count; s++) {
			var vars = Enumerable.Range(0, teachers.Count)
				.Where(t => x.ContainsKey((s, t)))
				.Select(t => x[(s, t)])
				.ToList();

			if (vars.Count == 0) {
				throw new InvalidOperationException(
					$"No trainer is qualified to teach block '{slots[s].Block.Name}' " +
					$"for group '{slots[s].Group.Name}' (week {slots[s].Week:yyyy-MM-dd}).");
			}

			model.AddExactlyOne(vars);
		}

		foreach (var weekGroup in slots.Select((slot, idx) => (slot, idx)).GroupBy(p => p.slot.Week)) {
			for (var t = 0; t < teachers.Count; t++) {
				var vars = weekGroup
					.Where(p => x.ContainsKey((p.idx, t)))
					.Select(p => x[(p.idx, t)])
					.ToList();

				if (vars.Count > 1) {
					model.AddAtMostOne(vars);
				}
			}
		}

		var preferenceTerms = new List<LinearExpr>();
		for (var s = 0; s < slots.Count; s++) {
			var preferredIndex = teachers.IndexOf(slots[s].Group.Teacher);
			if (preferredIndex >= 0 && x.ContainsKey((s, preferredIndex))) {
				preferenceTerms.Add(x[(s, preferredIndex)]);
			}
		}
		model.Maximize(LinearExpr.Sum(preferenceTerms));

		var solver = new CpSolver();
		var status = solver.Solve(model);

		if (status != CpSolverStatus.Optimal && status != CpSolverStatus.Feasible) {
			throw new InvalidOperationException(
				"No trainer assignment satisfies all constraints (qualification + no double-booking).");
		}

		for (var s = 0; s < slots.Count; s++) {
			var slot = slots[s];

			for (var t = 0; t < teachers.Count; t++) {
				if (x.ContainsKey((s, t)) && solver.Value(x[(s, t)]) == 1) {
					if (!plan.Assignments.TryGetValue(slot.Group, out var groupAssignments)) {
						groupAssignments = new Dictionary<DateTime, Assignment>();
						plan.Assignments[slot.Group] = groupAssignments;
					}

					groupAssignments[slot.Week] = new Assignment {
						Trainer = teachers[t].Abbreviation,
						BlockName = slot.Block.Name,
						BlockColor = slot.Block.Color
					};

					break;
				}
			}
		}

		return plan;
	}

	private sealed class Slot {
		public required Group Group {
			get; init;
		}
		public required Block Block {
			get; init;
		}
		public required DateTime Week {
			get; init;
		}
	}
}
