namespace Einsatzplanung.Input.Interfaces;

using System.Collections.Generic;

public interface IMappedConfigService<Tkey, TConfig> : IConfigService{

	public Dictionary<Tkey, List<TConfig>> ParseSource();

}