namespace Einsatzplanung.Input.Services;

using System.Linq;

using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models.Configuration;
using Einsatzplanung.Types.Models;

public class TeacherService : EntitiyServiceBase<Teacher, TeacherConfig> {

	private IEntityService<Topic, TopicConfig> topicService;

	public TeacherService(IConfigService<TeacherConfig> configService, IEntityService<Topic, TopicConfig> topicService) : base(configService) {
		this.topicService = topicService;
	}

	public override Teacher MapConfigData(TeacherConfig config) {
		return new Teacher() {
			Id = config.Id,
			Name = config.Name,
			Abbreviation = config.Abbreviation,
			WeeklyHours = config.WeeklyHours,
			Specializations = config.Specializations.Select(topicService.MapConfigData).ToList()
		};
	}
}