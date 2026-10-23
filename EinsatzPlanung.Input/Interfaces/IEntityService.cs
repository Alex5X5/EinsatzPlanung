namespace Einsatzplanung.Input.Interfaces;

using Einsatzplanung.Types.Models;
using Einsatzplanung.Types.Models.Configuration;

using System.Collections.Generic;

public interface IEntityService<Te, Tc> : IEntityService<Te> where Te : Entity where Tc : Config {

	public Te MapConfigData(Tc config);

}

public interface IEntityService<T> : IEntityService where T : Entity {

	public List<T> GetEntities();

}

public interface IEntityService {

	public void SetSource(string path);

}
