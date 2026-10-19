
using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models;
using Einsatzplanung.Types.Models.Configuration;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Einsatzplanung.Input.Services {

	public class TopicService : IEntityService<Topic> {

		private IConfigService<TopicConfig> configService;

		public TopicService(IConfigService<TopicConfig> configService) {
			this.configService = configService;
		}

		public List<Topic> GetEntities() {
			List<TopicConfig> configs = configService.GetData();
			return configs.Select(c => new Topic(c.Name, c.Color)).ToList();
		}

		public void SetSource(string path) {
			configService.SetSource(path);
		}
	}

}

namespace Einsatzplanung.Input.Interfaces {


	public static class TopicServiceExtensions {

		public static Topic GetTopicByName(this IEntityService<Topic> service, string name) {
			return service.GetEntities().First(t => t.Name.Equals(name, StringComparison.Ordinal));
		}

		public static bool TryGetTopicByName(this IEntityService<Topic> service, string name, out Topic? result) {

			if (string.IsNullOrEmpty(name)) {
				result = service.GetEntities().FirstOrDefault(t => t.Name.Equals(name, StringComparison.Ordinal));
			} else {
				result = null;
			}

			return result != null;
		}

	}

}