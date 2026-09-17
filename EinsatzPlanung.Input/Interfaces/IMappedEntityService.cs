namespace Einsatzplanung.Input.Interfaces;

using System.Collections.Generic;

public interface IMappedEntityService<Tkey, TConfig> {

	public Dictionary<Tkey, List<TConfig>> GetEntities();

}