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

	private const long PreferGroupTeacherWeight = 2;
	private const long PreferSameTeacherWeight = 3;

	private IEntityService<Group> groupService;
	private IEntityService<Teacher> teacherService;

	private readonly List<DateTime> weekStarts;

	private List<Group> Groups = [];
	private List<Teacher> Teachers = [];
	
	// Each group has one slot per week
	private List<Slot> Slots = [];

	// Whether a teacher is assigned to a slot
	private Dictionary<(int slot, int trainer), BoolVar> DoesTeachBySlotAndTeacher = [];

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
		var weekEnd = weekStart.AddDays(4);
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
			var blocksByLevel = group.Blocks
				.GroupBy(block => block.Topic.Level)
				.OrderBy(g => g.Key);
			
			var weekOffset = 0;

			foreach (var blocks in blocksByLevel) {

				var totalWeeksInLevel = blocks.Sum(block => block.Count);

				if (weekOffset + totalWeeksInLevel > validWeeks.Count)
					throw new InvalidOperationException($"Not enough remaining school weeks for level {blocks.Key} for group '{group.Name}'.");

				var levelWeeks = validWeeks.Skip(weekOffset).Take(totalWeeksInLevel).ToList();
				weekOffset += totalWeeksInLevel;

				foreach (var block in blocks) {
					if (block.Count == 0)
						continue;

					foreach (var week in levelWeeks) {
						slots.Add(new Slot {
							Group = group,
							Block = block,
							Week = week
						});
					}
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

	/// <summary>
	/// Makes sure that a <see cref="Group"/> is only assigned once per week
	/// </summary>
	private void AddOneBlockPerWeekConstraint() {
		var slotsByGroupAndWeek = Slots
			.Select((slot, index) => (slot, index))
			.GroupBy(p => (p.slot.Group, p.slot.Week));
		
		foreach (var slotsWithGroupAndWeek in slotsByGroupAndWeek) {
			var vars = slotsWithGroupAndWeek
				.SelectMany(indexedSlot => Enumerable
					.Range(0, Teachers.Count)
					.Where(teacherIndex => DoesTeachBySlotAndTeacher.ContainsKey((indexedSlot.index, teacherIndex)))
					.Select(teacherIndex => DoesTeachBySlotAndTeacher[(indexedSlot.index, teacherIndex)]))
				.ToList();

			if (vars.Count == 0) {
				throw new InvalidOperationException(
					$"No trainer is qualified for any block of group '{slotsWithGroupAndWeek.Key.Group.Name}' " +
					$"in week {slotsWithGroupAndWeek.Key.Week:yyyy-MM-dd}.");
			}

			Model.AddExactlyOne(vars);
		}
	}

	/// <summary>
	/// Makes sure that a <see cref="Group"/> is only assigned as many weeks with a <see cref="Topic"/> as is configured in the appropriate <see cref="Block"/>
	/// </summary>
	private void AddBlockCountConstraint() {
		var slotsByGroupBlock = Slots
			.Select((slot, index) => (slot, index))
			.GroupBy(p => (p.slot.Group, p.slot.Block));

		foreach (var slotsWithGroupAndBlock in slotsByGroupBlock) {
			var vars = slotsWithGroupAndBlock
				.SelectMany(indexedSlot => Enumerable
					.Range(0, Teachers.Count)
					.Where(teacherIndex => DoesTeachBySlotAndTeacher.ContainsKey((indexedSlot.index, teacherIndex)))
					.Select(teacherIndex => DoesTeachBySlotAndTeacher[(indexedSlot.index, teacherIndex)]))
				.ToList();

			if (vars.Count == 0) {
				throw new InvalidOperationException(
					$"No trainer is qualified to teach block '{slotsWithGroupAndBlock.Key.Block.Topic.Name}' " +
					$"for group '{slotsWithGroupAndBlock.Key.Group.Name}'.");
			}

			Model.Add(LinearExpr.Sum(vars) == slotsWithGroupAndBlock.Key.Block.Count);
		}
	}

	/// <summary>
	/// Makes sure that a <see cref="Teacher"/> is only assigned at most <see cref="MaxConcurrentGroupsPerTeacher"/> at once in a single week
	/// </summary>
	private void AddMaxParallelGroupsConstraint() {
		var slotsByWeek = Slots.Select((slot, index) => (slot, index)).GroupBy(p => p.slot.Week);
		foreach (var slotsAtWeek in slotsByWeek) {
			for (var teacherIndex = 0; teacherIndex < Teachers.Count; teacherIndex++) {
				var vars = slotsAtWeek
					.Where(slot => DoesTeachBySlotAndTeacher.ContainsKey((slot.index, teacherIndex)))
					.Select(slot => DoesTeachBySlotAndTeacher[(slot.index, teacherIndex)])
					.ToList();

				if (vars.Count > MaxConcurrentGroupsPerTeacher) {
					Model.Add(LinearExpr.Sum(vars) <= MaxConcurrentGroupsPerTeacher);
				}
			}
		}
	}

	/// <summary>
	/// Makes sure that all weeks of a <see cref="Group"/> with the same <see cref="Topic"/> are adjacent.
	/// School weeks and vacation weeks are allowed to interrupt the block.
	/// </summary>
	private void AddWholeBlockConstraint() {
		var slotsByGroupAndTopic = Slots
			.Select((slot, index) => (slot, index))
			.GroupBy(p => (p.slot.Group, p.slot.Block.Topic));

		var topicCounter = 0;

		foreach (var slotsWithGroupAndTopic in slotsByGroupAndTopic) {
			var weeks = slotsWithGroupAndTopic
				.GroupBy(p => p.slot.Week)
				.OrderBy(w => w.Key)
				.ToList();

			if (weeks.Count <= 2)
				continue;

			var prefix = $"topic{topicCounter++}";

			var topicActive = new List<BoolVar>();

			for (var i = 0; i < weeks.Count; i++) {
				var vars = weeks[i]
					.SelectMany(p => Enumerable
						.Range(0, Teachers.Count)
						.Where(teacherIndex => DoesTeachBySlotAndTeacher.ContainsKey((p.index, teacherIndex)))
						.Select(teacherIndex => DoesTeachBySlotAndTeacher[(p.index, teacherIndex)]))
					.ToList();

				var active = Model.NewBoolVar($"{prefix}_active_w{i}");

				if (vars.Count == 0) {
					Model.Add(active == 0);
				} else {
					Model.Add(active == LinearExpr.Sum(vars));
				}

				topicActive.Add(active);
			}

			var starts = new List<BoolVar>();

			for (var i = 0; i < weeks.Count; i++) {
				var start = Model.NewBoolVar($"{prefix}_start_w{i}");

				if (i == 0) {
					Model.Add(start >= topicActive[0]);
				} else {
					Model.Add(start >= topicActive[i] - topicActive[i - 1]);
				}

				starts.Add(start);
			}

			Model.Add(LinearExpr.Sum(starts) <= 1);
		}
	}

	#endregion

	#region objectives

	/// <summary>
	/// Prefer the <see cref="Group.Teacher"/> of a <see cref="Group"/> over others if possible
	/// </summary>
	private void AddPreferGroupTeacherObjective(List<LinearExpr> preferenceTerms) {
		for (var s = 0; s < Slots.Count; s++) {
			var preferredIndex = Teachers.IndexOf(Slots[s].Group.Teacher);
			if (preferredIndex >= 0 && DoesTeachBySlotAndTeacher.TryGetValue((s, preferredIndex), out var x)) {
				preferenceTerms.Add(LinearExpr.Term(x, PreferGroupTeacherWeight));
			}
		}
	}

	/// <summary>
	/// Prefer the same <see cref="Teacher"/> across all weeks of a <see cref="Group"/> with the same <see cref="Topic"/>.
	/// </summary>
	private void AddPreferSameTeacherObjective(List<LinearExpr> preferenceTerms) {
		var slotsByGroupAndTopic = Slots
			.Select((slot, index) => (slot, index))
			.GroupBy(p => (p.slot.Group, p.slot.Block.Topic));


		foreach (var slotsWithGroupAndTopic in slotsByGroupAndTopic) {
			var slotIndices = slotsWithGroupAndTopic.Select(p => p.index).ToList();

			// Nothing to prefer if the topic only occupies a single week
			if (slotIndices.Count <= 1)
				continue;

			for (var teacherIndex = 0; teacherIndex < Teachers.Count; teacherIndex++) {
				var teacherVars = slotIndices
					.Where(s => DoesTeachBySlotAndTeacher.ContainsKey((s, teacherIndex)))
					.Select(s => DoesTeachBySlotAndTeacher[(s, teacherIndex)])
					.ToList();

				if (teacherVars.Count == 0)
					continue;

				var teacherUsed = Model.NewBoolVar(
					$"used_{slotsWithGroupAndTopic.Key.Group.Id}_{slotsWithGroupAndTopic.Key.Topic.Name}_t{teacherIndex}");

				foreach (var x in teacherVars) {
					Model.AddImplication(x, teacherUsed);
				}

				preferenceTerms.Add(LinearExpr.Term(teacherUsed, -PreferSameTeacherWeight));
			}
		}
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
				if (DoesTeachBySlotAndTeacher.ContainsKey((slotIndex, teacherIndex)) && solver.Value(DoesTeachBySlotAndTeacher[(slotIndex, teacherIndex)]) == 1) {
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

		DoesTeachBySlotAndTeacher = GetBoolVariables();

		AddOneBlockPerWeekConstraint();
		AddBlockCountConstraint();
		AddMaxParallelGroupsConstraint();
		AddWholeBlockConstraint();

		var preferenceTerms = new List<LinearExpr>();

		AddPreferGroupTeacherObjective(preferenceTerms);
		AddPreferSameTeacherObjective(preferenceTerms);

		Model.Minimize(LinearExpr.Sum(preferenceTerms));

		var solver = new CpSolver();
		var status = solver.Solve(Model);

		if (status != CpSolverStatus.Optimal && status != CpSolverStatus.Feasible) {
			throw new InvalidOperationException("No trainer assignment satisfies all constraints.");
		}

		return CreatePlan(config, solver);
	}

	#endregion

	/// <summary>
	/// Represents a set week in which a <see cref="Teacher"/> can be assigned to a set <see cref="Types.Models.Group"/>.
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
