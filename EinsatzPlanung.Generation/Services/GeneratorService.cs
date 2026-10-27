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

	// ================= Einstellungen (hier oben anpassen) =================

	// Wie viele Gruppen darf ein Ausbilder in derselben Woche maximal gleichzeitig haben?
	// Das ist die harte Grenze. Der Solver versucht trotzdem, deutlich darunter zu bleiben.
	private const int MaxParallelClasses = 7;

	// Wie viele Wochen darf ein Thema von seiner Wunschposition (Eingabereihenfolge) abweichen?
	// 0 = Reihenfolge ist fest, so wie beim alten Generator.
	private const int MaxShiftWeeks = 3;

	// Maximale Rechenzeit des Solvers in Sekunden.
	private const int SolveTimeLimitSeconds = 30;

	// true  = ausgeblendete Gruppen werden mitgeplant (belegen Ausbilder), aber nicht exportiert.
	// false = ausgeblendete Gruppen werden komplett ignoriert.
	private static readonly bool PlanHiddenGroupsToo = true;

	// ================= Gewichte (je höher, desto wichtiger) =================
	// Der Solver sucht den Plan mit den niedrigsten Gesamtkosten.

	// Pro Woche, die nicht eingeplant werden konnte. Muss mit Abstand am höchsten sein.
	private const long WeightMissingWeek = 10_000;

	// Pro zusätzlichem Ausbilder innerhalb eines Themas (Wechsel mitten im Thema).
	private const long WeightTeacherSwitch = 40;

	// Pro Woche, in der NICHT der zuständige Ausbilder der Gruppe unterrichtet.
	private const long WeightForeignTeacher = 30;

	// Zweite gleichzeitige Gruppe eines Ausbilders kostet 1x, die dritte 2x, die vierte 3x ...
	private const long WeightParallelStep = 20;

	// Pro Woche, die ein Thema von seiner Wunschposition verschoben wird.
	// Bewusst höher als eine einzelne Doppelbelegung (20): Eine zweite Gruppe beim selben Ausbilder
	// ist besser als ein Thema zu verschieben. Erst bei starker Überlast wird verschoben.
	private const long WeightShiftWeek = 25;

	// Gleichmäßige Verteilung der "fremden" Einsätze auf die Ausbilder.
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

	// Eine Woche ist nur gesperrt, wenn alle 5 Arbeitstage (Mo-Fr) im Urlaub oder Feiertag liegen.
	// Ein einzelner Feiertag mitten in der Woche sperrt die Woche also nicht.
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

	// Erzeugt für jede Gruppe alle sinnvollen "Kandidaten":
	// "Thema X könnte in Woche Y laufen". Der Solver wählt später daraus aus.
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

			// Wunschposition: Die Themen liegen hintereinander in den freien Wochen,
			// genau wie beim alten Generator. blockStart ist der Index in validWeeks.
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

		// Alles, was "schlecht" ist, wird hier als Kosten gesammelt. Am Ende wird die Summe minimiert.
		var objective = new List<LinearExpr>();

		// ---------- Schritt 1: Variablen ----------
		// Pro Kandidat und qualifiziertem Ausbilder eine Ja/Nein-Variable:
		// "Ausbilder t unterrichtet dieses Thema in dieser Woche für diese Gruppe."
		for (var c = 0; c < candidates.Count; c++) {
			var cand = candidates[c];
			var responsibleIndex = teachers.IndexOf(cand.Group.Teacher);

			for (var t = 0; t < teachers.Count; t++) {
				if (!teachers[t].CanTeachBlock(cand.Block)) {
					continue;
				}

				var assign = model.NewBoolVar($"x_c{c}_t{t}");
				cand.TeacherVars[t] = assign;

				// Kosten: Thema verschoben + nicht der zuständige Ausbilder
				long cost = WeightShiftWeek * cand.Distance;

				if (responsibleIndex >= 0 && t != responsibleIndex) {
					cost += WeightForeignTeacher;
				}

				if (cost > 0) {
					objective.Add(cost * assign);
				}
			}
		}

		// ---------- Schritt 2: Eine Gruppe hat pro Woche höchstens ein Thema mit einem Ausbilder ----------
		foreach (var perGroupWeek in candidates.GroupBy(c => (c.Group, c.Week))) {
			var vars = perGroupWeek.SelectMany(c => c.TeacherVars.Values).ToList();

			if (vars.Count > 1) {
				model.AddAtMostOne(vars);
			}
		}

		// ---------- Schritt 3: Jedes Thema bekommt genau seine Wochenanzahl ----------
		// Was nicht passt, landet in "missing" (sehr teuer) und wird am Ende gemeldet,
		// statt dass das ganze Programm abbricht.
		var missingVars = new List<(Group Group, Block Block, IntVar Missing)>();

		foreach (var perBlock in candidates.GroupBy(c => (c.Group, c.BlockIndex))) {
			var firstCand = perBlock.First();
			var vars = perBlock.SelectMany(c => c.TeacherVars.Values).ToList();

			var missing = model.NewIntVar(0, firstCand.Block.Count, $"missing_{firstCand.BlockIndex}");
			model.Add(SumOf(vars) + missing == firstCand.Block.Count);

			// Frühe Themen sind minimal wichtiger als späte
			var weight = WeightMissingWeek + (firstCand.BlockCount - firstCand.BlockIndex);
			objective.Add(weight * missing);

			missingVars.Add((firstCand.Group, firstCand.Block, missing));
		}

		// ---------- Schritt 4: Ausbilder in mehreren Gruppen gleichzeitig ----------
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

			// Harte Grenze
			if (vars.Count > MaxParallelClasses) {
				model.Add(load <= MaxParallelClasses);
			}

			// Weiche Kosten: Stufe k ist "mehr als k Gruppen gleichzeitig".
			// Die Kosten steigen mit jeder Stufe, dadurch verteilt der Solver die Last.
			for (var k = 1; k < Math.Min(vars.Count, MaxParallelClasses); k++) {
				var above = model.NewBoolVar($"over_t{key.Teacher}_{key.Week:yyyyMMdd}_{k}");
				model.Add(load <= k).OnlyEnforceIf(above.Not());
				objective.Add(WeightParallelStep * k * above);
			}
		}

		// ---------- Schritt 5: Möglichst kein Ausbilderwechsel innerhalb eines Themas ----------
		foreach (var perBlock in candidates.GroupBy(c => (c.Group, c.BlockIndex))) {
			var usedBy = new Dictionary<int, BoolVar>();

			foreach (var cand in perBlock) {
				foreach (var (t, assign) in cand.TeacherVars) {
					if (!usedBy.TryGetValue(t, out var used)) {
						used = model.NewBoolVar($"used_t{t}");
						usedBy[t] = used;

						// Jeder Ausbilder, der im Thema vorkommt, kostet. Ein zweiter ist also teurer.
						objective.Add(WeightTeacherSwitch * used);
					}

					model.Add(assign <= used);
				}
			}
		}

		// ---------- Schritt 6: "Fremde" Einsätze gleichmäßig verteilen ----------
		// Fremd = der Ausbilder ist nicht der zuständige der Gruppe.
		// Wir minimieren die Wochenzahl des am meisten eingesetzten Fremd-Ausbilders.
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

		// ---------- Schritt 7: Lösen ----------
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

		// ---------- Schritt 8: Ergebnis in den Plan schreiben ----------
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

		// ---------- Schritt 9: Hinweise sammeln ----------
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

	// "Thema X könnte in Woche Y für Gruppe Z laufen"
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

		// Abstand zur Wunschposition in Wochen (0 = genau dort, wo es nach Eingabereihenfolge hingehört)
		public required int Distance {
			get; init;
		}

		// Ausbilder-Index -> Variable "unterrichtet das"
		public Dictionary<int, BoolVar> TeacherVars {
			get;
		} = new();
	}
}