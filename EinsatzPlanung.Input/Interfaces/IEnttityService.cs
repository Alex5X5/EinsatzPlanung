namespace EinsatzPlanung.Generation.Interfaces;

using System.Collections.Generic;

using Einsatzplanung.Excel.Models;

public interface IEnttityService<T> {
	
	public List<T> ParseExcelTable(Table table);

}
