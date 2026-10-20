namespace Einsatzplanung.Input.Services;

using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models;
using Einsatzplanung.Types.Models.Configuration;
using Einsatzplanung.Util.Services;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

public class AgeGroupService : EntitiyServiceBase<AgeGroup, AgeGroupConfig> {

	private IEntityService<Holiday> holidayService;
	private IEntityService<Teacher> teacherService;
	private IEntityService<Topic> topicService;
	private GeneralConfigService generalConfigService;

	public AgeGroupService(IConfigService<AgeGroupConfig> ageGroupService, IEntityService<Holiday> holidayService, IEntityService<Teacher> teacherService, IEntityService<Topic> topicService, GeneralConfigService generalConfigService)
	: base(ageGroupService) {
		this.holidayService = holidayService;
		this.teacherService = teacherService;
		this.topicService = topicService;
		this.generalConfigService = generalConfigService;
	}

	protected override AgeGroup MapConfigData(AgeGroupConfig config) {
		return new AgeGroup() {
			Name = config.Name,
			Groups = config.Groups.Select(MapGroup).ToList()
		};
	}

	private Group MapGroup(GroupConfig config) {
		var blocks = new List<Block>();

		var currentStart = generalConfigService.YearStartDate;

		blocks = config.Blocks
			.Where(x => x.Count > 0)
			.Select(MapBlock)
			.ToList();
		
		var teachers = teacherService.GetEntities();
		return new Group() {
			Name = config.Name,
			AgeGroupName = config.AgeGroupName,
			Teacher = teachers.First(x=>x.Abbreviation == config.TeacherAbbreviation),
			SchoolWeeks = config.SchoolWeeks,
			Blocks = blocks,
			Holidays = holidayService.GetEntities()
		};
	}

	private Block MapBlock(BlockConfig config) {
		string topicName = config.Topic.Name;
		var topic = topicService.GetEntities().FirstOrDefault(x => x.Name == topicName);
		if (topic != null) {
			return new Block {
				Topic = topic,
				Count = config.Count
			};
		} else {
			throw new ValidationException($"Topic '{config.Topic}' not found.");
		}
	}
}
