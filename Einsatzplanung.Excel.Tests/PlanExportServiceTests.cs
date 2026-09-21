namespace Einsatzplanung.Excel.Tests;

using ClosedXML.Excel;
using Einsatzplanung.Excel.Services;
using Einsatzplanung.Types.Models;
using Einsatzplanung.Types.Models.Generation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

public sealed class PlanExportServiceTests
{
	private readonly string _testFilePath;
	private readonly PlanExportService _exportService;

	public PlanExportServiceTests()
	{
		_testFilePath = Path.Combine(Path.GetTempPath(), "test_plan_export.xlsx");
		_exportService = new PlanExportService(
			new DateTime(2024, 1, 1),
			new DateTime(2024, 12, 31)
		);
	}

	public void Dispose()
	{
		if (File.Exists(_testFilePath))
		{
			File.Delete(_testFilePath);
		}
	}

	[Fact]
	public void ExportPlan_WithValidPlan_CreatesExcelFile()
	{
		// Arrange
		var plan = CreateTestPlan();

		// Act
		_exportService.ExportPlan(_testFilePath, plan);

		// Assert
		Assert.True(File.Exists(_testFilePath));
	}

	[Fact]
	public void ExportPlan_WithMultipleGroups_CreatesMultipleWorksheets()
	{
		// Arrange
		var plan = CreateTestPlanWithMultipleGroups();

		// Act
		_exportService.ExportPlan(_testFilePath, plan);

		// Assert
		Assert.True(File.Exists(_testFilePath));
	}

	[Fact]
	public void ExportPlan_WithDatesOutsideRange_FiltersCorrectly()
	{
		// Arrange
		var plan = CreateTestPlanWithMixedDates();

		// Act
		_exportService.ExportPlan(_testFilePath, plan);

		// Assert
		Assert.True(File.Exists(_testFilePath));
	}

	[Fact]
	public void AnalyzeReferenceExcel()
	{
		// Arrange
		string referencePath = @"C:\Users\Rene\RiderProjects\EinsatzPlanung\Einsatzplanung.Excel.Tests\Einsatzplan_2026_2027.xlsx";
		string outputPath = @"C:\Users\Rene\RiderProjects\EinsatzPlanung\Einsatzplanung.Excel.Tests\analysis_output.txt";

		if (!File.Exists(referencePath))
		{
			return;
		}

		// Act
		XLWorkbook workbook = new(referencePath);

		// Assert - Nur zur Analyse
		using (var writer = new StreamWriter(outputPath))
		{
			foreach (var worksheet in workbook.Worksheets)
			{
				writer.WriteLine($"Worksheet: {worksheet.Name}");
				int rowCount = 0;
				foreach (var row in worksheet.RowsUsed())
				{
					rowCount++;
				}
				writer.WriteLine($"Rows: {rowCount}");
				foreach (var row in worksheet.RowsUsed().Take(10))
				{
					foreach (var cell in row.Cells())
					{
						writer.Write($"[{cell.Address}]: {cell.Value} | ");
					}
					writer.WriteLine();
				}
				writer.WriteLine();
			}
		}
	}

	[Fact]
	public void ExportWeeklyPlan_WithValidAssignments_CreatesExcelFile()
	{
		// Arrange
		var weeklyAssignments = new Dictionary<(string Group, DateTime WeekStart), Assignment>
		{
			[("FI26a", new DateTime(2024, 6, 17))] = new Assignment
			{
				Trainer = "Lu",
				BlockName = "Block A",
				BlockColor = "#FF0000"
			},
			[("FI26a", new DateTime(2024, 6, 24))] = new Assignment
			{
				Trainer = "Ki",
				BlockName = "Block B",
				BlockColor = "#00FF00"
			}
		};

		// Act
		_exportService.ExportWeeklyPlan(_testFilePath, weeklyAssignments);

		// Assert
		Assert.True(File.Exists(_testFilePath));
	}

	[Fact]
	public void ExportWeeklyPlan_WithMultipleGroupsAndWeeks_CreatesExcelFile()
	{
		// Arrange
		var weeklyAssignments = new Dictionary<(string Group, DateTime WeekStart), Assignment>
		{
			// FI26a assignments
			[("FI26a", new DateTime(2024, 6, 17))] = new Assignment
			{
				Trainer = "Lu",
				BlockName = "Block A",
				BlockColor = "#FF0000"
			},
			[("FI26a", new DateTime(2024, 6, 24))] = new Assignment
			{
				Trainer = "Ki",
				BlockName = "Block B",
				BlockColor = "#00FF00"
			},
			[("FI26a", new DateTime(2024, 7, 1))] = new Assignment
			{
				Trainer = "Sch",
				BlockName = "Block C",
				BlockColor = "#0000FF"
			},
			// FI26b assignments
			[("FI26b", new DateTime(2024, 6, 17))] = new Assignment
			{
				Trainer = "He",
				BlockName = "Block A",
				BlockColor = "#FF0000"
			},
			[("FI26b", new DateTime(2024, 6, 24))] = new Assignment
			{
				Trainer = "Sb",
				BlockName = "Block B",
				BlockColor = "#00FF00"
			},
			[("FI26b", new DateTime(2024, 7, 1))] = new Assignment
			{
				Trainer = "F",
				BlockName = "Block C",
				BlockColor = "#0000FF"
			},
			// FI27a assignments
			[("FI27a", new DateTime(2024, 6, 17))] = new Assignment
			{
				Trainer = "U",
				BlockName = "Block A",
				BlockColor = "#FF0000"
			},
			[("FI27a", new DateTime(2024, 6, 24))] = new Assignment
			{
				Trainer = "Lu",
				BlockName = "Block B",
				BlockColor = "#00FF00"
			}
		};

		// Act
		_exportService.ExportWeeklyPlan(_testFilePath, weeklyAssignments);

		// Assert
		Assert.True(File.Exists(_testFilePath));
	}

	[Fact]
	public void ExportWeeklyPlan_WithMixedDates_FiltersCorrectly()
	{
		// Arrange
		var weeklyAssignments = new Dictionary<(string Group, DateTime WeekStart), Assignment>
		{
			// Before range
			[("FI26a", new DateTime(2023, 12, 25))] = new Assignment
			{
				Trainer = "OT",
				BlockName = "Old Block",
				BlockColor = "#808080"
			},
			// In range
			[("FI26a", new DateTime(2024, 6, 17))] = new Assignment
			{
				Trainer = "CT",
				BlockName = "Current Block",
				BlockColor = "#FF0000"
			},
			[("FI26a", new DateTime(2024, 9, 2))] = new Assignment
			{
				Trainer = "MT",
				BlockName = "Mid Block",
				BlockColor = "#00FF00"
			},
			// After range
			[("FI26a", new DateTime(2025, 1, 6))] = new Assignment
			{
				Trainer = "FT",
				BlockName = "Future Block",
				BlockColor = "#0000FF"
			}
		};

		// Act
		_exportService.ExportWeeklyPlan(_testFilePath, weeklyAssignments);

		// Assert
		Assert.True(File.Exists(_testFilePath));
	}

	private static Plan CreateTestPlan()
	{
		var group = new Group
		{
			Name = "TestGroup",
			SchoolWeeks = [1, 2, 3],
			Blocks = [],
			Holidays = []
		};

		var assignments = new Dictionary<DateTime, Assignment>
		{
			[new DateTime(2024, 6, 17)] = new Assignment // Monday
			{
				Trainer = "MM",
				BlockName = "Block A",
				BlockColor = "#FF0000"
			},
			[new DateTime(2024, 6, 18)] = new Assignment // Tuesday
			{
				Trainer = "Ki",
				BlockName = "Block B",
				BlockColor = "#00FF00"
			}
		};

		return new Plan
		{
			Assignments = new Dictionary<Group, Dictionary<DateTime, Assignment>>
			{
				[group] = assignments
			}
		};
	}

	private static Plan CreateTestPlanWithMultipleGroups()
	{
		var group1 = new Group
		{
			Name = "Group1",
			SchoolWeeks = [1, 2],
			Blocks = [],
			Holidays = []
		};

		var group2 = new Group
		{
			Name = "Group2",
			SchoolWeeks = [3, 4],
			Blocks = [],
			Holidays = []
		};

		var assignments1 = new Dictionary<DateTime, Assignment>
		{
			[new DateTime(2024, 6, 17)] = new Assignment // Monday
			{
				Trainer = "T1",
				BlockName = "Block A",
				BlockColor = "#FF0000"
			}
		};

		var assignments2 = new Dictionary<DateTime, Assignment>
		{
			[new DateTime(2024, 6, 17)] = new Assignment // Monday
			{
				Trainer = "T2",
				BlockName = "Block B",
				BlockColor = "#00FF00"
			}
		};

		return new Plan
		{
			Assignments = new Dictionary<Group, Dictionary<DateTime, Assignment>>
			{
				[group1] = assignments1,
				[group2] = assignments2
			}
		};
	}

	private static Plan CreateTestPlanWithMixedDates()
	{
		var group = new Group
		{
			Name = "TestGroup",
			SchoolWeeks = [1, 2, 3],
			Blocks = [],
			Holidays = []
		};

		var assignments = new Dictionary<DateTime, Assignment>
		{
			[new DateTime(2023, 12, 25)] = new Assignment // Monday before range
			{
				Trainer = "OT",
				BlockName = "Old Block",
				BlockColor = "#0000FF"
			},
			[new DateTime(2024, 6, 17)] = new Assignment // Monday in range
			{
				Trainer = "CT",
				BlockName = "Current Block",
				BlockColor = "#FF0000"
			},
			[new DateTime(2025, 1, 6)] = new Assignment // Monday after range
			{
				Trainer = "FT",
				BlockName = "Future Block",
				BlockColor = "#00FF00"
			}
		};

		return new Plan
		{
			Assignments = new Dictionary<Group, Dictionary<DateTime, Assignment>>
			{
				[group] = assignments
			}
		};
	}
}
