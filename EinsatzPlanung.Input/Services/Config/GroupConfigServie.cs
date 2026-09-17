namespace Einsatzplanung.Input.Services.Config;

using System.Collections.Generic;

using Einsatzplanung.Input.Interfaces;
using System.Linq;
using Einsatzplanung.Types.Models.Configuration;

public class GroupConfigService : IConfigService<GroupConfig> {

	private IConfigService<AgeGroupConfig> ageGroupService;

	public GroupConfigService(IConfigService<AgeGroupConfig> ageGroupService) {
		this.ageGroupService = ageGroupService;
	}

	public void SetSource(string path) {
		ageGroupService.SetSource(path);
	}

	public List<GroupConfig> ParseSource() {
		return ageGroupService.ParseSource().SelectMany((group) => group.Groups).ToList();
	}

}
