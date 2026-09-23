namespace Einsatzplanung.Input.Services;

using System.Collections.Generic;
using System.Linq;

using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models;
using Einsatzplanung.Types.Models.Configuration;
using Einsatzplanung.Util.Services;

public class AgeGroupService : IEntityService<AgeGroup> {

	private IConfigService<AgeGroupConfig> ageGroupService;
	private IEntityService<Holiday> holidayService;
	private IEntityService<Teacher> teacherService;
	private GeneralConfigService configService;

	public AgeGroupService(IConfigService<AgeGroupConfig> ageGroupService, IEntityService<Holiday> holidayService, IEntityService<Teacher> teacherService, GeneralConfigService configService) {
		this.ageGroupService = ageGroupService;
		this.holidayService = holidayService;
		this.teacherService = teacherService;
		this.configService = configService;
	}

	public List<AgeGroup> GetEntities() {
		List<AgeGroupConfig> configs = ageGroupService.ParseSource();
		return configs.Select(MapAgeGroup).ToList();
	}

	public void SetSource(string path) {
		ageGroupService.SetSource(path);
	}

	private AgeGroup MapAgeGroup(AgeGroupConfig config) {
		return new AgeGroup() {
			Name = config.Name,
			Groups = config.Groups.Select(MapGroup).ToList()
		};
	}

	private Group MapGroup(GroupConfig config) {
		var blocks = new List<Block>();

		var currentStart = configService.YearStartDate;

		blocks = config.Blocks
			.Where(x => x.Count > 0)
			.Select(MapBlock)
			.ToList();

		return new Group() {
			Name = config.Name,
			Teacher = teacherService.GetEntities().First(x=>x.Abbreviation == config.TeacherAbbreviation),
			SchoolWeeks = config.SchoolWeeks,
			Blocks = blocks,
			Holidays = holidayService.GetEntities()
		};
	}

	private Block MapBlock(BlockConfig config) {
		return new Block {
			Name = config.Name,
			Count = config.Count,
			Color = config.Color
		};
	}
}
