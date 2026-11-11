namespace Einsatzplanung.Input.Services; 

using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models;
using Einsatzplanung.Types.Models.Configuration;

public class TopicService : EntitiyServiceBase<Topic, TopicConfig> {

	public TopicService(IConfigService<TopicConfig> configService) : base(configService) { }

	public override Topic MapConfigData(TopicConfig config) {
		return new Topic() {
			Id = config.Id,
			Name = config.Name,
			Level = int.TryParse(config.Level, out var level) ? level : 0,
			Color = config.Color
		};
	}
}