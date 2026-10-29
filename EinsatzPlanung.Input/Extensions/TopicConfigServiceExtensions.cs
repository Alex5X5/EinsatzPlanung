namespace Einsatzplanung.Input.Interfaces;

using System.Linq;
using System.Threading.Tasks;

using Einsatzplanung.Types;
using Einsatzplanung.Types.Models.Configuration;

public static class TopicConfigServiceExtensions {

	public static async Task<TopicConfig?> GetByNameAsync(this IConfigService<TopicConfig> service, string name) {
		var data = await service.GetDataAsync();
		return data.FirstOrDefault(x => x.Name == name);
	}

	public static async Task<TryResult<TopicConfig>> TryGetByNameAsync(this IConfigService<TopicConfig> service, string name) {
		var config = await service.GetByNameAsync(name);
		return new TryResult<TopicConfig>(config != null, config);
	}
}
