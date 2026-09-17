namespace Einsatzplanung.Input.Services;

using System.Collections.Generic;

using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models;
using Einsatzplanung.Types.Models.Configuration;

using GroupConfigService = Interfaces.IConfigService<Types.Models.Configuration.GroupConfig>;
using BlockConfigService = Interfaces.IMappedConfigService<Types.Models.Configuration.GroupConfig, Types.Models.Configuration.BlockConfig>;

public class BlockService : IMappedEntityService<Group, Block> {

	private GroupConfigService groupConfigService;
	private BlockConfigService blockConfigService;

	public BlockService(GroupConfigService groupConfigService, BlockConfigService blockConfigService) {
		this.groupConfigService = groupConfigService;
		this.blockConfigService = blockConfigService;
	}

	public Dictionary<Group, List<Block>> GetEntities() {
		
		Dictionary<Group, List<Block>> blocks = [];
		Dictionary<GroupConfig, List<BlockConfig>> blockConfigs = blockConfigService.ParseSource();
		
		foreach (var groupConfig in groupConfigService.ParseSource()) {
			
		}

		return blocks;
	}
}
