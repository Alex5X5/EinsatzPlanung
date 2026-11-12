namespace Einsatzplanung.Input.Interfaces;

using System.Collections.Generic;
using System.Threading.Tasks;

using Einsatzplanung.Types.Models;
using Einsatzplanung.Types.Models.Configuration;

public interface IEntityService<Te, Tc> : IEntityService<Te> where Te : Entity where Tc : Config {

	public Task<Te> MapConfigDataAsync(Tc config);

}

public interface IEntityService<T> : IEntityService where T : Entity {

	public Task<List<T>> GetEntitiesAsync();

}

public interface IEntityService {

	public void SetSource(string path);

}
