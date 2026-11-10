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
	private const int MaxConcurrentGroupsPerTeacher = 2;

	private IEntityService<Group> groupService;
	private IEntityService<Teacher> teacherService;

	private readonly List<DateTime> weekStarts;

	private List<Slot> Slots = [];
	private List<Group> Groups = [];
	private List<Teacher> Teachers = [];
	private Dictionary<(int slot, int trainer), BoolVar> BoolVairables = [];

	public SolverGeneratorService(IEntityService<Group> groupService, IEntityService<Teacher> teacherService, GeneralConfigService configService) {
		this.groupService = groupService;
		this.teacherService = teacherService;
		weekStarts = DateTimeService.GetWeekStartsInIntervall(configService.YearStartDate, configService.YearEndDate);
	}

	private static bool IsSchoolWeek(Group group, DateTime date) {
		return group.SchoolWeeks.Contains(DateTimeService.WeekOfYear(date));
	}

	private static bool IsVacationWeek(Group group, DateTime weekStart) {
		var weekEnd = weekStart.AddDays(5);
		return group.Holidays.Any(
			period =>
				period.From <= DateOnly.FromDateTime(weekStart) &&
				period.To >= DateOnly.FromDateTime(weekEnd));
	}

	private static bool ShouldExport(GeneratorConfig config, Group group) {
		return config.GroupsToExport != null
			&& config.GroupsToExport.TryGetValue(group.Id, out var export)
			&& export;
	}

	private List<Slot> GetSlots() {
		var slots = new List<Slot>();

		foreach (var group in Groups) {
			var validWeeks = weekStarts
				.Where(week => !IsSchoolWeek(group, week) && !IsVacationWeek(group, week))
				.OrderBy(week => week)
				.ToList();

			var weekIndex = 0;

			foreach (var block in group.Blocks) {
				for (var i = 0; i < block.Count; i++) {
					if (weekIndex >= validWeeks.Count) {
						throw new InvalidOperationException(
							$"Not enough remaining school weeks for block '{block.Topic.Name}' in group '{group.Name}'.");
					}

					slots.Add(new Slot {
						Group = group,
						Block = block,
						Week = validWeeks[weekIndex++]
					});
				}
			}
		}

		return slots;
	}

	private Dictionary<(int slot, int trainer), BoolVar> GetBoolVariables(CpModel model) {
		Dictionary<(int slot, int trainer), BoolVar> vars = [];
		for (var s = 0; s < Slots.Count; s++) {
			for (var t = 0; t < Teachers.Count; t++) {
				if (Teachers[t].CanTeachBlock(Slots[s].Block)) {
					vars[(s, t)] = model.NewBoolVar($"x_s{s}_t{t}");
				}
			}
		}
		return vars;
	}

	private void AddCanTeachConstraint(CpModel model) {
		for (var s = 0; s < Slots.Count; s++) {
			var vars = Enumerable.Range(0, Teachers.Count)
				.Where(t => BoolVairables.ContainsKey((s, t)))
				.Select(t => BoolVairables[(s, t)])
				.ToList();

			if (vars.Count == 0) {
				throw new InvalidOperationException(
					$"No trainer is qualified to teach block '{Slots[s].Block.Topic.Name}' " +
					$"for group '{Slots[s].Group.Name}' (week {Slots[s].Week:yyyy-MM-dd}).");
			}

			model.AddExactlyOne(vars);
		}
	}

	private void AddMaxParallelGroupsConstraint(CpModel model) {
		var slotsByWeek = Slots.Select((slot, index) => (slot, index)).GroupBy(p => p.slot.Week);
		foreach (var slotsAtWeek in slotsByWeek) {
			for (var teacherIndex = 0; teacherIndex < Teachers.Count; teacherIndex++) {
				var vars = slotsAtWeek
					.Where(slot => BoolVairables.ContainsKey((slot.index, teacherIndex)))
					.Select(slot => BoolVairables[(slot.index, teacherIndex)])
					.ToList();

				if (vars.Count > MaxConcurrentGroupsPerTeacher) {
					model.Add(LinearExpr.Sum(vars) <= MaxConcurrentGroupsPerTeacher);
				}
			}
		}
	}

	public Plan GeneratePlan(GeneratorConfig config) {
		Groups = groupService.GetEntities();
		Teachers = teacherService.GetEntities();

		var Slots = GetSlots();

		var model = new CpModel();

		BoolVairables = GetBoolVariables(model);

		AddCanTeachConstraint(model);
		AddMaxParallelGroupsConstraint(model);

		var preferenceTerms = new List<LinearExpr>();
		for (var s = 0; s < Slots.Count; s++) {
			var preferredIndex = Teachers.IndexOf(Slots[s].Group.Teacher);
			if (preferredIndex >= 0 && BoolVairables.ContainsKey((s, preferredIndex))) {
				preferenceTerms.Add(BoolVairables[(s, preferredIndex)]);
			}
		}
		model.Maximize(LinearExpr.Sum(preferenceTerms));

		var solver = new CpSolver();
		var status = solver.Solve(model);

		if (status != CpSolverStatus.Optimal && status != CpSolverStatus.Feasible) {
			throw new InvalidOperationException("No trainer assignment satisfies all constraints.");
		}

		var plan = new Plan();

		for (var s = 0; s < Slots.Count; s++) {
			var slot = Slots[s];

			if (!ShouldExport(config, slot.Group))
				continue;

			for (var t = 0; t < Teachers.Count; t++) {
				if (BoolVairables.ContainsKey((s, t)) && solver.Value(BoolVairables[(s, t)]) == 1) {
					if (!plan.Assignments.TryGetValue(slot.Group, out var groupAssignments)) {
						groupAssignments = new Dictionary<DateTime, Assignment>();
						plan.Assignments[slot.Group] = groupAssignments;
					}

					groupAssignments[slot.Week] = new Assignment {
						Trainer = Teachers[t].Abbreviation,
						BlockName = slot.Block.Topic.Name,
						BlockColor = slot.Block.Topic.Color
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

	private record class PlanGenerationContext(CpModel Model, List<Group> Groups, List<Teacher> Teachers, List<Slot> Slots, Dictionary<(int slot, int trainer), BoolVar> BoolVairables);
}
