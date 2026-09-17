namespace Einsatzplanung.Input.Services;

using Avalonia.Controls;

using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models;
using Einsatzplanung.Types.Models.Configuration;

using System;
using System.Collections.Generic;
using System.Linq;

public class GroupService : IEntityService<Group> {

	private IConfigService<GroupConfig> configService;

	public GroupService(IConfigService<GroupConfig> configService) {
		this.configService = configService;
	}

	public List<Group> GetEntities() {
		List<GroupConfig> configs = configService.ParseSource();
		List<Group> groups = [];

		foreach (var config in configs) {
			var currentStart = new DateTime(2000,1,1);

			var blocks = new List<Block>();

			foreach (var block in config.Blocks) {
				if (block.Anzahl <= 0)
					continue;

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
		}
		
		return groups;
	}
}
