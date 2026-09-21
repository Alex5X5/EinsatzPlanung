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

	private AgeGroup MapAgeGroup(AgeGroupConfig config) {
		return new AgeGroup() {
			Name = config.Name,
			Groups = config.Groups.Select(MapGroup).ToList()
		};
	}

	private Group MapGroup(GroupConfig config) {
		var blocks = new List<Block>();

		var currentStart = configService.YearStartDate;

		foreach (var block in config.Blocks) {
			if (block.Anzahl <= 0) {
				continue;
			}

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

		return new Group() {
			Name = config.Name,
			Teacher = teacherService.GetEntities().First(x=>x.Abbreviation == config.TeacherAbbreviation),
			SchoolWeeks = config.SchoolWeeks,
			Blocks = blocks,
			Holidays = holidayService.GetEntities()
		};
	}
}
