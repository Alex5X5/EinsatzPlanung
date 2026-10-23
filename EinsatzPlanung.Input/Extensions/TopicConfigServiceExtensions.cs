namespace Einsatzplanung.Input.Interfaces;

using System.Linq;

using Einsatzplanung.Types.Models.Configuration;

public static class TopicConfigServiceExtensions {

	public static TopicConfig? GetByName(this IConfigService<TopicConfig> service, string name) {
		var topics = service.GetData();
		return service
			.GetData()
			.FirstOrDefault(x => x.Name == name);
	}

	public static bool TryGetByName(this IConfigService<TopicConfig> service, string name, out TopicConfig? topic) {
		var topics = service.GetData();
		topic = service
			.GetData()
			.First(x => x.Name == name);
		return topic != null;
	}
}
