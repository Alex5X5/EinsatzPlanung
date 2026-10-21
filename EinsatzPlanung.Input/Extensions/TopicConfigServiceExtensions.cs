namespace Einsatzplanung.Input.Interfaces;

using System;
using System.Linq;

using Einsatzplanung.Types.Models.Configuration;

public static class TopicConfigServiceExtensions {

	public static TopicConfig GetByName(this IConfigService<TopicConfig> service, string name) {
		var topics = service.GetData();
		return service
			.GetData()
			.First(x => x.Name == name);
	}
}
