namespace Einsatzplanung.Input.Services;

using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models.Configuration;
using Einsatzplanung.Types.Models;

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

public class TeacherService : IEntityService<Teacher> {

	private IConfigService<TeacherConfig> configService;

	public TeacherService(IConfigService<TeacherConfig> configService) {
		this.configService = configService;
	}

	public List<Teacher> GetEntities() {
		return configService.ParseSource().Select(MapTeacher).ToList();
	}

	private static Teacher MapTeacher(TeacherConfig config) =>
		new Teacher() {
			Name = config.Name,
			Abbreviation = config.Abbreviation,
			WeeklyHours = config.WeeklyHours,
			Specializations = config.Specializations
		};
}
