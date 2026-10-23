namespace Einsatzplanung.Input.Interfaces;

using System.Linq;

using Einsatzplanung.Types.Models.Configuration;

public static class TeacherServiceExtensions {

	public static TeacherConfig? GetByAbbreviation(this IConfigService<TeacherConfig> service, string abbreviation) {
		return service
			.GetData()
			.FirstOrDefault(x => x.Abbreviation == abbreviation);
	}

	public static bool TryGetByAbbreviation(this IConfigService<TeacherConfig> service, string abbreviation, out TeacherConfig? config) {
		config = service.GetByAbbreviation(abbreviation);
		return config != null;
	}
}
