namespace Einsatzplanung.Input.Interfaces;

using System.Collections.Generic;

public interface IMappedEntityService<Tkey, TConfig> where Tkey : notnull {

	public Dictionary<Tkey, List<TConfig>> GetEntities();

}