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

	private List<Group> Groups = [];
	private List<Teacher> Teachers = [];
	
	// Each group has one slot per week
	private List<Slot> Slots = [];

	// Whether a teacher is assigned to a slot
	private Dictionary<(int slot, int trainer), BoolVar> BoolVairables = [];

	private CpModel Model;

	public SolverGeneratorService(IEntityService<Group> groupService, IEntityService<Teacher> teacherService, GeneralConfigService configService) {
		this.groupService = groupService;
		this.teacherService = teacherService;
		weekStarts = DateTimeService.GetWeekStartsInIntervall(configService.YearStartDate, configService.YearEndDate);
	}

	#region helpers

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

	private static bool IsVacationDay(Group group, DateTime weekStart) {
		return group.Holidays.Any(
			period =>
				period.From <= DateOnly.FromDateTime(weekStart) &&
				period.To >= DateOnly.FromDateTime(weekStart));
	}

	private static bool ShouldExport(GeneratorConfig config, Group group) {
		return config.GroupsToExport != null
			&& config.GroupsToExport.TryGetValue(group.Id, out var export)
			&& export;
	}

	#endregion

	#region constraint model setup

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

	private Dictionary<(int slot, int trainer), BoolVar> GetBoolVariables() {
		Dictionary<(int slot, int trainer), BoolVar> vars = [];
		for (var s = 0; s < Slots.Count; s++) {
			for (var t = 0; t < Teachers.Count; t++) {
				if (Teachers[t].CanTeachBlock(Slots[s].Block)) {
					vars[(s, t)] = Model.NewBoolVar($"x_s{s}_t{t}");
				}
			}
		}
		return vars;
	}

	#endregion

	#region constraints

	private void AddCanTeachConstraint() {
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

			Model.AddExactlyOne(vars);
		}
	}

	private void AddMaxParallelGroupsConstraint() {
		var slotsByWeek = Slots.Select((slot, index) => (slot, index)).GroupBy(p => p.slot.Week);
		foreach (var slotsAtWeek in slotsByWeek) {
			for (var teacherIndex = 0; teacherIndex < Teachers.Count; teacherIndex++) {
				var vars = slotsAtWeek
					.Where(slot => BoolVairables.ContainsKey((slot.index, teacherIndex)))
					.Select(slot => BoolVairables[(slot.index, teacherIndex)])
					.ToList();

				if (vars.Count > MaxConcurrentGroupsPerTeacher) {
					Model.Add(LinearExpr.Sum(vars) <= MaxConcurrentGroupsPerTeacher);
				}
			}
		}
	}

	private void AddPreferGroupTeacherConstraint() {
		var preferenceTerms = new List<LinearExpr>();
		for (var s = 0; s < Slots.Count; s++) {
			var preferredIndex = Teachers.IndexOf(Slots[s].Group.Teacher);
			if (preferredIndex >= 0 && BoolVairables.ContainsKey((s, preferredIndex))) {
				preferenceTerms.Add(BoolVairables[(s, preferredIndex)]);
			}
		}
		Model.Maximize(LinearExpr.Sum(preferenceTerms));
	}

	#endregion

	#region plan creation

	private Plan CreatePlan(GeneratorConfig config, CpSolver solver) {
		var plan = new Plan();
		for (var slotIndex = 0; slotIndex < Slots.Count; slotIndex++) {
			var slot = Slots[slotIndex];

			if (!ShouldExport(config, slot.Group))
				continue;

			for (var teacherIndex = 0; teacherIndex < Teachers.Count; teacherIndex++) {
				if (BoolVairables.ContainsKey((slotIndex, teacherIndex)) && solver.Value(BoolVairables[(slotIndex, teacherIndex)]) == 1) {
					if (!plan.Assignments.TryGetValue(slot.Group, out var groupAssignments)) {
						groupAssignments = new Dictionary<DateTime, Assignment>();
						plan.Assignments[slot.Group] = groupAssignments;
					}

					groupAssignments[slot.Week] = new Assignment {
						Trainer = Teachers[teacherIndex].Abbreviation,
						BlockName = slot.Block.Topic.Name,
						BlockColor = slot.Block.Topic.Color,
						IsHoliday = Enumerable
							.Range(0, 5)
							.Select(x => IsVacationDay(slot.Group, slot.Week.AddDays(x)))
							.ToArray()
					};

					break;
				}
			}
		}
		return plan;
	}

	public Plan GeneratePlan(GeneratorConfig config) {
		Groups = groupService.GetEntities();
		Teachers = teacherService.GetEntities();

		Slots = GetSlots();

		Model = new();

		BoolVairables = GetBoolVariables();

		AddCanTeachConstraint();
		AddMaxParallelGroupsConstraint();
		AddPreferGroupTeacherConstraint();

		var solver = new CpSolver();
		var status = solver.Solve(Model);

		if (status != CpSolverStatus.Optimal && status != CpSolverStatus.Feasible) {
			throw new InvalidOperationException("No trainer assignment satisfies all constraints.");
		}

		return CreatePlan(config, solver);
	}

	#endregion

	/// <summary>
	/// Represents a week in which a <see cref="Teacher"/> can be assigned to a <see cref="Types.Models.Group"/>.
	/// </summary>
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
