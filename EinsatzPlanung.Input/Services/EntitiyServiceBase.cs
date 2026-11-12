namespace Einsatzplanung.Input.Services;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models;
using Einsatzplanung.Types.Models.Configuration;

public abstract class EntitiyServiceBase<Te, Tc> : IEntityService<Te, Tc> where Te : Entity where Tc : Config {

	public IConfigService<Tc> configService;

	public EntitiyServiceBase(IConfigService<Tc> configService) {
		this.configService = configService;
	}

	public void SetSource(string path) {
		configService.SetSource(path);
	}

	public async Task<List<Te>> GetEntitiesAsync() {
		List<Tc> configs = await configService.GetDataAsync();
		return (await Task.WhenAll(configs.Select(async x => await MapConfigDataAsync(x)))).ToList();
	}

	//public Te? GetById(Guid id) {
	//	throw new NotImplementedException();
	//}

	//public bool TryGetById(Guid id, out Te? result) {
	//	List<Te> configs = GetEntities();
	//	result = configs.FirstOrDefault(x => x.Id == id);
	//}

	public abstract Task<Te> MapConfigDataAsync(Tc config);
}
