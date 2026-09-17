namespace Einsatzplanung.Input.Services;

using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models;
using Einsatzplanung.Types.Models.Configuration;

using System.Collections.Generic;

public class AgeGroupService : IEntityService<AgeGroup> {

	private IConfigService<AgeGroupConfig> configService;

	public AgeGroupService(IConfigService<AgeGroupConfig> configService) {
		this.configService = configService;
	}

	public List<AgeGroup> GetEntities() {
		List<AgeGroupConfig> configs = configService.ParseSource();
		return [];
	}
}
