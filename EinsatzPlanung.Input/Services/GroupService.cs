namespace Einsatzplanung.Input.Services;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models;

public class GroupService : IEntityService<Group> {

	private IEntityService<AgeGroup> ageGroupService;

	public GroupService(IEntityService<AgeGroup> ageGroupService) {
		this.ageGroupService = ageGroupService;
	}

	public async Task<List<Group>> GetEntitiesAsync() {
		var ageGroups = await ageGroupService.GetEntitiesAsync();
		return ageGroups.SelectMany((group) => group.Groups).ToList();
	}

	public void SetSource(string path) {
		ageGroupService.SetSource(path);
	}
}
