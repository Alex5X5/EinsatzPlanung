namespace Einsatzplanung.Input.Services;

using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models.Configuration;
using Einsatzplanung.Types.Models;

using System.Linq;

public class TeacherService : EntitiyServiceBase<Teacher, TeacherConfig> {

	private IEntityService<Topic> topicService;

	public TeacherService(IConfigService<TeacherConfig> configService, IEntityService<Topic> topicService)
	: base(configService) {
		this.topicService = topicService;
	}

	protected override Teacher MapConfigData(TeacherConfig config) {
		return new Teacher() {
			Name = config.Name,
			Abbreviation = config.Abbreviation,
			WeeklyHours = config.WeeklyHours,
			Specializations = config.Specializations.Select(MapTopic).ToList()
		};
	}

	private Topic MapTopic(TopicConfig config) {
		return topicService.GetEntities().FirstOrDefault(x => x.Name == config.Name, Topic.Default);
	}
}