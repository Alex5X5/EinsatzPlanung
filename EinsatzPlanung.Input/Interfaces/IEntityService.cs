namespace EinsatzPlanung.Generation.Interfaces;

using System.Collections.Generic;

using Einsatzplanung.Excel.Models;

public interface IEntityService<T> {
	
	public List<T> ParseExcelTable(Table table);

}
