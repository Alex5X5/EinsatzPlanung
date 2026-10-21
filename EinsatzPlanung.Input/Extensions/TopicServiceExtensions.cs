namespace Einsatzplanung.Input.Interfaces;

using System.Linq;

using Einsatzplanung.Types.Models;

public static class TopicServiceExtensions {

	public static Topic GetByName(this IEntityService<Topic> service, string name) {
		return service
			.GetEntities()
			.FirstOrDefault(x => x.Name == name, Topic.Default);
	}
}
