namespace Einsatzplanung.Input.Interfaces;

using System.Collections.Generic;

public interface IMappedConfigService<Tkey, TConfig> : IConfigService where Tkey : notnull{

	public Dictionary<Tkey, List<TConfig>> ParseSource();

}