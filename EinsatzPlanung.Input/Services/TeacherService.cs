namespace EinsatzPlanung.Input.Services;

using System.Collections.Generic;

using EinsatzPlanung.Generation.Interfaces;

using Einsatzplanung.Types.Models;
using Einsatzplanung.Excel.Models;
using System;

public class TeacherService : IEntityService<Teacher> {

	public TeacherService() {

	}

	public List<Teacher> ParseExcelTable(Table table) {
		Console.WriteLine("Helloppppp Worlllllld");
		return [];
	}
}
