namespace EinsatzPlanung.Input.Services;

using System.Collections.Generic;

using Einsatzplanung.Types.Models;
using EinsatzPlanung.Input.Interfaces;

public class GroupService : IEntityService<Group> {

	public GroupService() {

	}

	public List<Group> ParseSource() {
		return [];
	}

	public void SetSource(string path) {
	}
}
