namespace EinsatzPlanung.Generation.Services;

using System.Collections.Generic;

using EinsatzPlanung.Generation.Interfaces;
using Einsatzplanung.Types.Models;
using Einsatzplanung.Excel.Models;

public class BlockService : IEnttityService<Block> {

	public BlockService() {

	}

	public List<Block> ParseExcelTable(Table table) {
		return [];
	}
}
