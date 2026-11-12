namespace Einsatzplanung.Input.Interfaces;

using System.Linq;
using System.Threading.Tasks;

using Einsatzplanung.Types.Models;

public static class TopicServiceExtensions {

	public static async Task<Topic> GetByNameAsync(this IEntityService<Topic> service, string name) {
		return (await service.GetEntitiesAsync())
			.FirstOrDefault(x => x.Name == name, Topic.Default);
	}
}
