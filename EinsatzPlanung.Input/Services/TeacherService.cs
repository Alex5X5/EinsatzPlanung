namespace EinsatzPlanung.Input.Services;

using System.Collections.Generic;

using Einsatzplanung.Types.Models;
using Einsatzplanung.Excel.Models;
using System;
using EinsatzPlanung.Input.Interfaces;

public class TeacherService : IEntityService<Teacher> {

	public TeacherService() {

	}

	public List<Teacher> ParseExcelTable(Table table) {
		Console.WriteLine("Helloppppp Worlllllld");
		return [];
	}

	public List<Teacher> ParseSource() {
		throw new NotImplementedException();
	}

	public void SetSource(string path) {
		throw new NotImplementedException();
	}
}
