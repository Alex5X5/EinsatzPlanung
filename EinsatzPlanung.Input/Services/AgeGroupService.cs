namespace Einsatzplanung.Input.Services;

using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models;
using Einsatzplanung.Types.Models.Configuration;
using Einsatzplanung.Util.Services;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

public class AgeGroupService : EntitiyServiceBase<AgeGroup, AgeGroupConfig> {

	private IEntityService<Holiday, HolidayConfig> holidayService;
	private IEntityService<Teacher, TeacherConfig> teacherService;
	private IEntityService<Topic, TopicConfig> topicService;
	private GeneralConfigService generalConfigService;

	public AgeGroupService(
		IConfigService<AgeGroupConfig> ageGroupService,
		IEntityService<Holiday, HolidayConfig> holidayService,
		IEntityService<Teacher, TeacherConfig> teacherService,
		IEntityService<Topic, TopicConfig> topicService,
		GeneralConfigService generalConfigService)
	: base(ageGroupService) {
		this.holidayService = holidayService;
		this.teacherService = teacherService;
		this.topicService = topicService;
		this.generalConfigService = generalConfigService;
	}

	public override async Task<AgeGroup> MapConfigDataAsync(AgeGroupConfig config) {
		return new AgeGroup() {
			Id = config.Id,
			Name = config.Name,
			Groups = (await Task.WhenAll(config.Groups.Select(MapGroupAsync))).ToList()
		};
	}

	private async Task<Group> MapGroupAsync(GroupConfig config) {
		var blocks = new List<Block>();

		var currentStart = generalConfigService.YearStartDate;

		blocks = (await Task.WhenAll(config.Blocks.Where(x => x.Count > 0).Select(MapBlockAsync))).ToList();
		
		return new Group() {
			Id = config.Id,
			Name = config.Name,
			Teacher = await teacherService.MapConfigDataAsync(config.Teacher),
			SchoolWeeks = config.SchoolWeeks,
			Blocks = blocks,
			Holidays = await holidayService.GetEntitiesAsync()
		};
	}

	private async Task<Block> MapBlockAsync(BlockConfig config) {
		string topicName = config.Topic.Name;
		return new Block {
			Id = config.Id,
			Topic = await topicService.MapConfigDataAsync(config.Topic),
			Count = config.Count
		};
	}
}
