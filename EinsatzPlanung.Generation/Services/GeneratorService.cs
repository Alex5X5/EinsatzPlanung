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

public class GeneratorService : IGeneratorService {

	// ================= Settings =================

	// Hard limit of groups a trainer may teach in parallel (solver still keeps it as low as possible)
	private const int MaxParallelClasses = 7;

	// How many weeks a topic may be shifted from its preferred position (0 = fixed order, may cause problems)
	private const int MaxShiftWeeks = 5;

	// Solver time limit in seconds
	private const int SolveTimeLimitSeconds = 30;

	// true  = hidden groups are planned (they occupy trainers) but not exported
	// false = hidden groups are ignored completely
	private static readonly bool PlanHiddenGroupsToo = true;

	// ================= Weights (higher = more important) =================
	// The solver looks for the plan with the lowest total cost.

	// Per week that could not be scheduled. Must be by far the highest.
	private const long WeightMissingWeek = 10_000;

	// Per additional trainer within one topic (switch mid-topic)
	private const long WeightTeacherSwitch = 40;

	// Per week taught by someone other than the group's own trainer
	private const long WeightForeignTeacher = 30;

	// 2nd parallel group of a trainer costs 1x, 3rd costs 2x, 4th 3x ...
	private const long WeightParallelStep = 20;

	// Per week a topic is shifted from its preferred position.
	// Deliberately higher than a double booking (20): a 2nd group for the same trainer
	// is better than shifting a topic. Shifting only happens under heavy overload.
	private const long WeightShiftWeek = 25;

	// Even distribution of "foreign" assignments across trainers
	private const long WeightBalance = 3;

	// ======================================================================

	private IEntityService<Group> groupService;
	private IEntityService<Teacher> teacherService;

	private readonly List<DateTime> weekStarts;

	public GeneratorService(IEntityService<Group> groupService, IEntityService<Teacher> teacherService, GeneralConfigService configService) {
		this.groupService = groupService;
		this.teacherService = teacherService;
		weekStarts = DateTimeService.GetWeekStartsInIntervall(configService.YearStartDate, configService.YearEndDate);
	}

	private static bool IsSchoolWeek(Group group, DateTime date) {
		return group.SchoolWeeks.Contains(DateTimeService.WeekOfYear(date));
	}

	// A week is blocked only if all 5 workdays (Mon-Fri) are vacation or holiday.
	private static bool IsVacationWeek(Group group, DateTime weekStart) {
		for (var d = 0; d < 5; d++) {
			var day = DateOnly.FromDateTime(weekStart.AddDays(d));
			var covered = group.Holidays.Any(period => period.From <= day && period.To >= day);

			if (!covered) {
				return false;
			}
		}

		return true;
	}

	private static bool ShouldExport(GeneratorConfig config, Group group) {
		return config.GroupsToExport != null
			&& config.GroupsToExport.TryGetValue(group.Id, out var export)
			&& export;
	}

	private static LinearExpr SumOf(IEnumerable<BoolVar> vars) {
		var terms = new List<LinearExpr>();

		foreach (var v in vars) {
			terms.Add(v);
		}

		return LinearExpr.Sum(terms);
	}

	private static void PrintNotes(List<string> notes) {
		if (notes.Count == 0) {
			return;
		}

		Console.WriteLine("=== Hinweise zur Planerstellung ===");

		foreach (var note in notes) {
			Console.WriteLine("- " + note);
		}
	}

	// Builds all candidates per group: "topic X could run in week Y". The generator picks from them.
	private List<Candidate> BuildCandidates(List<Group> groups, List<Teacher> teachers, GeneratorConfig config, List<string> notes) {
		var candidates = new List<Candidate>();

		foreach (var group in groups) {
			if (!PlanHiddenGroupsToo && !ShouldExport(config, group)) {
				continue;
			}

			var validWeeks = weekStarts
				.Where(week => !IsSchoolWeek(group, week) && !IsVacationWeek(group, week))
				.OrderBy(week => week)
				.ToList();

			var blocks = group.Blocks.ToList();

			// Preferred position: topics run back to back in the free weeks (like the old generator).
			// blockStart is an index into validWeeks.
			var blockStart = 0;

			for (var b = 0; b < blocks.Count; b++) {
				var block = blocks[b];
				var blockEnd = blockStart + block.Count;
				var visible = ShouldExport(config, group);

				if (!teachers.Any(teacher => teacher.CanTeachBlock(block))) {
					if (visible) {
						notes.Add($"Gruppe '{group.Name}': Kein Ausbilder kann das Thema '{block.Topic.Name}' unterrichten, {block.Count} Wochen bleiben leer.");
					}

					blockStart = blockEnd;
					continue;
				}

				var firstIndex = Math.Max(0, blockStart - MaxShiftWeeks);
				var lastIndex = Math.Min(validWeeks.Count - 1, blockEnd - 1 + MaxShiftWeeks);
				var created = 0;

				for (var i = firstIndex; i <= lastIndex; i++) {
					var distance = 0;

					if (i < blockStart) {
						distance = blockStart - i;
					} else if (i >= blockEnd) {
						distance = i - (blockEnd - 1);
					}

					candidates.Add(new Candidate {
						Group = group,
						Block = block,
						BlockIndex = b,
						BlockCount = blocks.Count,
						Week = validWeeks[i],
						Distance = distance
					});
					created++;
				}

				if (created == 0 && visible) {
					notes.Add($"Gruppe '{group.Name}': Für das Thema '{block.Topic.Name}' sind keine freien Wochen mehr übrig.");
				}

				blockStart = blockEnd;
			}
		}

		return candidates;
	}

	public Plan GeneratePlan(GeneratorConfig config) {
		var plan = new Plan();
		var notes = new List<string>();

		List<Group> groups = groupService.GetEntities();
		List<Teacher> teachers = teacherService.GetEntities();

		var candidates = BuildCandidates(groups, teachers, config, notes);

		if (candidates.Count == 0) {
			PrintNotes(notes);
			return plan;
		}

		var model = new CpModel();

		// All costs are collected here; the sum is minimized.
		var objective = new List<LinearExpr>();

		// ---------- Step 1: Variables ----------
		// One bool per candidate and qualified trainer: "trainer t teaches this topic in this week for this group"
		for (var c = 0; c < candidates.Count; c++) {
			var cand = candidates[c];
			var responsibleIndex = teachers.IndexOf(cand.Group.Teacher);

			for (var t = 0; t < teachers.Count; t++) {
				if (!teachers[t].CanTeachBlock(cand.Block)) {
					continue;
				}

				var assign = model.NewBoolVar($"x_c{c}_t{t}");
				cand.TeacherVars[t] = assign;

				// Cost: topic shifted + not the responsible trainer
				long cost = WeightShiftWeek * cand.Distance;

				if (responsibleIndex >= 0 && t != responsibleIndex) {
					cost += WeightForeignTeacher;
				}

				if (cost > 0) {
					objective.Add(cost * assign);
				}
			}
		}

		// ---------- Step 2: At most one topic/trainer per group and week ----------
		foreach (var perGroupWeek in candidates.GroupBy(c => (c.Group, c.Week))) {
			var vars = perGroupWeek.SelectMany(c => c.TeacherVars.Values).ToList();

			if (vars.Count > 1) {
				model.AddAtMostOne(vars);
			}
		}

		// ---------- Step 3: Each topic gets exactly its number of weeks ----------
		// Whatever doesn't fit goes into "missing" (very expensive) and is reported at the end.
		var missingVars = new List<(Group Group, Block Block, IntVar Missing)>();

		foreach (var perBlock in candidates.GroupBy(c => (c.Group, c.BlockIndex))) {
			var firstCand = perBlock.First();
			var vars = perBlock.SelectMany(c => c.TeacherVars.Values).ToList();

			var missing = model.NewIntVar(0, firstCand.Block.Count, $"missing_{firstCand.BlockIndex}");
			model.Add(SumOf(vars) + missing == firstCand.Block.Count);

			// Early topics are slightly more important than late ones
			var weight = WeightMissingWeek + (firstCand.BlockCount - firstCand.BlockIndex);
			objective.Add(weight * missing);

			missingVars.Add((firstCand.Group, firstCand.Block, missing));
		}

		// ---------- Step 4: Trainer teaching several groups at once ----------
		var loadByTeacherWeek = new Dictionary<(int Teacher, DateTime Week), List<BoolVar>>();

		foreach (var cand in candidates) {
			foreach (var (t, assign) in cand.TeacherVars) {
				var key = (t, cand.Week);

				if (!loadByTeacherWeek.TryGetValue(key, out var list)) {
					list = new List<BoolVar>();
					loadByTeacherWeek[key] = list;
				}

				list.Add(assign);
			}
		}

		foreach (var (key, vars) in loadByTeacherWeek) {
			if (vars.Count <= 1) {
				continue;
			}

			var load = SumOf(vars);

			// Hard limit
			if (vars.Count > MaxParallelClasses) {
				model.Add(load <= MaxParallelClasses);
			}

			// Soft cost: level k = "more than k groups at once". Cost grows per level, so load gets spread out.
			for (var k = 1; k < Math.Min(vars.Count, MaxParallelClasses); k++) {
				var above = model.NewBoolVar($"over_t{key.Teacher}_{key.Week:yyyyMMdd}_{k}");
				model.Add(load <= k).OnlyEnforceIf(above.Not());
				objective.Add(WeightParallelStep * k * above);
			}
		}

		// ---------- Step 5: Avoid trainer switches within a topic ----------
		foreach (var perBlock in candidates.GroupBy(c => (c.Group, c.BlockIndex))) {
			var usedBy = new Dictionary<int, BoolVar>();

			foreach (var cand in perBlock) {
				foreach (var (t, assign) in cand.TeacherVars) {
					if (!usedBy.TryGetValue(t, out var used)) {
						used = model.NewBoolVar($"used_t{t}");
						usedBy[t] = used;

						// Every trainer appearing in the topic costs, so a second one is more expensive
						objective.Add(WeightTeacherSwitch * used);
					}

					model.Add(assign <= used);
				}
			}
		}

		// ---------- Step 6: Spread "foreign" assignments evenly ----------
		// Foreign = trainer is not the group's responsible trainer.
		// Minimizes the weeks of the most-used foreign trainer.
		var foreignByTeacher = new Dictionary<int, List<BoolVar>>();

		foreach (var cand in candidates) {
			var responsibleIndex = teachers.IndexOf(cand.Group.Teacher);

			foreach (var (t, assign) in cand.TeacherVars) {
				if (t == responsibleIndex) {
					continue;
				}

				if (!foreignByTeacher.TryGetValue(t, out var list)) {
					list = new List<BoolVar>();
					foreignByTeacher[t] = list;
				}

				list.Add(assign);
			}
		}

		var foreignMax = model.NewIntVar(0, candidates.Count, "foreign_max");

		foreach (var list in foreignByTeacher.Values) {
			model.Add(SumOf(list) <= foreignMax);
		}

		objective.Add(WeightBalance * foreignMax);

		// ---------- Step 7: Solve ----------
		model.Minimize(LinearExpr.Sum(objective));

		var solver = new CpSolver();
		solver.StringParameters = $"max_time_in_seconds:{SolveTimeLimitSeconds}";
		var status = solver.Solve(model);

		if (status != CpSolverStatus.Optimal && status != CpSolverStatus.Feasible) {
			throw new InvalidOperationException(
				"Der Solver konnte keinen Plan berechnen (Status: " + status + ").");
		}

		if (status == CpSolverStatus.Feasible) {
			notes.Add($"Zeitlimit von {SolveTimeLimitSeconds}s erreicht: Der Plan ist gut, aber evtl. nicht der beste.");
		}

		// ---------- Step 8: Write result into the plan ----------
		foreach (var cand in candidates) {
			if (!ShouldExport(config, cand.Group)) {
				continue;
			}

			foreach (var (t, assign) in cand.TeacherVars) {
				if (solver.Value(assign) != 1) {
					continue;
				}

				if (!plan.Assignments.TryGetValue(cand.Group, out var groupAssignments)) {
					groupAssignments = new Dictionary<DateTime, Assignment>();
					plan.Assignments[cand.Group] = groupAssignments;
				}

				groupAssignments[cand.Week] = new Assignment {
					Trainer = teachers[t].Abbreviation,
					BlockName = cand.Block.Topic.Name,
					BlockColor = cand.Block.Topic.Color
				};

				break;
			}
		}

		// ---------- Step 9: Collect notes ----------
		foreach (var (group, block, missing) in missingVars) {
			var open = solver.Value(missing);

			if (open > 0 && ShouldExport(config, group)) {
				notes.Add($"Gruppe '{group.Name}', Thema '{block.Topic.Name}': {open} von {block.Count} Wochen konnten nicht eingeplant werden.");
			}
		}

		foreach (var perBlock in candidates.GroupBy(c => (c.Group, c.BlockIndex))) {
			var firstCand = perBlock.First();

			if (!ShouldExport(config, firstCand.Group)) {
				continue;
			}

			var usedTeachers = perBlock
				.SelectMany(c => c.TeacherVars.Where(p => solver.Value(p.Value) == 1).Select(p => p.Key))
				.Distinct()
				.ToList();

			if (usedTeachers.Count > 1) {
				var names = string.Join(", ", usedTeachers.Select(t => teachers[t].Abbreviation));
				notes.Add($"Gruppe '{firstCand.Group.Name}', Thema '{firstCand.Block.Topic.Name}': Ausbilderwechsel innerhalb des Themas ({names}).");
			}

			var shiftedWeeks = perBlock.Count(c => c.Distance > 0 && c.TeacherVars.Values.Any(v => solver.Value(v) == 1));

			if (shiftedWeeks > 0) {
				notes.Add($"Gruppe '{firstCand.Group.Name}', Thema '{firstCand.Block.Topic.Name}': {shiftedWeeks} Woche(n) von der Wunschposition verschoben.");
			}
		}

		foreach (var perTeacher in loadByTeacherWeek.GroupBy(p => p.Key.Teacher)) {
			var loads = perTeacher.Select(p => p.Value.Count(v => solver.Value(v) == 1)).ToList();
			var parallelWeeks = loads.Count(l => l > 1);

			if (parallelWeeks > 0) {
				notes.Add($"Ausbilder '{teachers[perTeacher.Key].Abbreviation}': in {parallelWeeks} Woche(n) für mehrere Gruppen gleichzeitig eingeplant (maximal {loads.Max()}).");
			}
		}

		PrintNotes(notes);

		return plan;
	}

	// "Topic X could run in week Y for group Z"
	private sealed class Candidate {
		public required Group Group {
			get; init;
		}
		public required Block Block {
			get; init;
		}
		public required int BlockIndex {
			get; init;
		}
		public required int BlockCount {
			get; init;
		}
		public required DateTime Week {
			get; init;
		}

		// Distance in weeks from the preferred position (0 = exactly where input order puts it)
		public required int Distance {
			get; init;
		}

		// Trainer index -> variable "teaches this"
		public Dictionary<int, BoolVar> TeacherVars {
			get;
		} = new();
	}
}