namespace Einsatzplanung.Input.Interfaces;

using System.Collections.Generic;

public interface IConfigService<T> : IConfigService {

	public List<T> ParseSource();

}

public interface IConfigService {

	public void SetSource(string path);

}
