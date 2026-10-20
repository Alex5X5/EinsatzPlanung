namespace Einsatzplanung.Input.Services;

using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models;
using Einsatzplanung.Types.Models.Configuration;

using System;
using System.Collections.Generic;
using System.Linq;

public abstract class EntitiyServiceBase<Te, Tc> : IEntityService<Te> where Te : Entity where Tc : Config {

	public IConfigService<Tc> configService;

	public EntitiyServiceBase(IConfigService<Tc> configService) {
		this.configService = configService;
	}

	public void SetSource(string path) {
		configService.SetSource(path);
	}

	public List<Te> GetEntities() {
		List<Tc> configs = configService.GetData();
		return configs.Select(MapConfigData).ToList();
	}

	public Te? GetById(int id) {
		throw new NotImplementedException();
	}

	public bool TryGetById(int id, out Te? result) {
		throw new NotImplementedException();
	}

	protected abstract Te MapConfigData(Tc config);
}
