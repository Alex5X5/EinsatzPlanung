namespace Einsatzplanung.Excel.Interfaces;

using Einsatzplanung.Types.Models;
using Einsatzplanung.Types.Models.Generation;
using System;
using System.Collections.Generic;

public interface IPlanExportService
{
	void ExportPlan(string filePath, Plan plan);

	void ExportWeeklyPlan(string filePath, Dictionary<(string Group, DateTime WeekStart), Assignment> weeklyAssignments);
}
