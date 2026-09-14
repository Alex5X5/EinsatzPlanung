namespace EinsatzPlanung.Input.Services;

using System.Collections.Generic;

using Einsatzplanung.Types.Models;
using Einsatzplanung.Excel.Models;

using EinsatzPlanung.Input.Interfaces;

public class BlockService : IEntityService<Block> {

	public BlockService() {
		
	}

	public List<Block> ParseSource() {
		throw new System.NotImplementedException();
	}

	public void SetSource(string path) {
		throw new System.NotImplementedException();
	}
}
