namespace EinsatzPlanung.Generation.Services;

using System.Collections.Generic;

using EinsatzPlanung.Generation.Interfaces;

using Einsatzplanung.Types.Models;
using Einsatzplanung.Excel.Models;

public class TeacherService : IEnttityService<Teacher> {

	public TeacherService() {

	}

	public List<Teacher> ParseExcelTable(Table table) {
		return [];
	}
}
