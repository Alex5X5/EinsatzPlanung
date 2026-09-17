namespace Einsatzplanung.Input.Services.Config;

using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models.Configuration;

using System.Collections.Generic;
using System.Linq;

public class BlockConfigService : IMappedConfigService<GroupConfig, BlockConfig> {

	private readonly IConfigService<GroupConfig> groupService;

	public BlockConfigService(IConfigService<GroupConfig> groupService) {
		this.groupService = groupService;
	}

	public void SetSource(string path) {
		groupService.SetSource(path);
	}

	public Dictionary<GroupConfig, List<BlockConfig>> ParseSource() {
		var groups = groupService.ParseSource();
		return groups.ToDictionary(group => group, group => group.Blocks);
	}
}
