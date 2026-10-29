namespace Einsatzplanung.Input.Interfaces;

using System.Linq;
using System.Threading.Tasks;

using Einsatzplanung.Types;
using Einsatzplanung.Types.Models.Configuration;

public static class TeacherServiceExtensions {

	public static async Task<TeacherConfig?> GetByAbbreviationAsync(this IConfigService<TeacherConfig> service, string abbreviation) {
		var data = await service.GetDataAsync();
		return data.FirstOrDefault(x => x.Abbreviation == abbreviation);
	}

	public static async Task<TryResult<TeacherConfig>> TryGetByAbbreviation(this IConfigService<TeacherConfig> service, string abbreviation) {
		var config = await service.GetByAbbreviationAsync(abbreviation);
		return new TryResult<TeacherConfig>(config != null, config);
	}
}
