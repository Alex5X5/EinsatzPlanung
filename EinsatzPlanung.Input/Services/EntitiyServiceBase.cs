namespace Einsatzplanung.Input.Services;

using System;
using System.Collections.Generic;
using System.Linq;

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

	public List<Te> GetEntities() {
		List<Tc> configs = configService.GetDataAsync().Result;
		return configs.Select(MapConfigData).ToList();
	}

	//public Te? GetById(Guid id) {
	//	throw new NotImplementedException();
	//}

	//public bool TryGetById(Guid id, out Te? result) {
	//	List<Te> configs = GetEntities();
	//	result = configs.FirstOrDefault(x => x.Id == id);
	//}

	public abstract Te MapConfigData(Tc config);
}
