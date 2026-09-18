namespace Einsatzplanung.Input.Services;

using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models;
using Einsatzplanung.Types.Models.Configuration;

using System;
using System.Collections.Generic;
using System.Linq;

public class AgeGroupService : IEntityService<AgeGroup> {

	private IConfigService<AgeGroupConfig> configService;
	private IEntityService<Holiday> holidayService;

	public AgeGroupService(IConfigService<AgeGroupConfig> configService, IEntityService<Holiday> holidayService) {
		this.configService = configService;
	}

	public List<AgeGroup> GetEntities() {
		List<AgeGroupConfig> configs = configService.ParseSource();
		return configs.Select(MapAgeGroup).ToList();
	}

	private AgeGroup MapAgeGroup(AgeGroupConfig config) {
		return new AgeGroup() {
			Name = config.Name,
			Groups = config.Groups.Select(MapGroup).ToList()
		};
	}

	private Group MapGroup(GroupConfig config) {
		var blocks = new List<Block>();

		var currentStart = new DateTime(1,1,2025);
		//config.SchuljahrStart.Date;

		foreach (var block in config.Blocks) {
			if (block.Anzahl <= 0) {
				continue;
			}

			var from = currentStart;
			var to = currentStart.AddDays(block.Anzahl * 7 - 1);

			blocks.Add(new Block {
				Name = block.Name,
				From = from,
				To = to,
				Color = block.Color
			});

			currentStart = to.AddDays(1);
		}

		return new Group() {
			Name = config.Name,
			SchoolWeeks = config.SchoolWeeks,
			Blocks = [],
			Holidays = holidayService.GetEntities()
		};
	}
}
