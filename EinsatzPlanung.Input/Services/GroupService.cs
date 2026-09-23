namespace Einsatzplanung.Input.Services;

using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models;

using System.Collections.Generic;
using System.Linq;

public class GroupService : IEntityService<Group> {

	private IEntityService<AgeGroup> ageGroupService;

	public GroupService(IEntityService<AgeGroup> ageGroupService) {
		this.ageGroupService = ageGroupService;
	}

	public List<Group> GetEntities() {
		return ageGroupService.GetEntities().SelectMany((group) => group.Groups).ToList();
	}

	public void SetSource(string path) {
		ageGroupService.SetSource(path);
	}
}
