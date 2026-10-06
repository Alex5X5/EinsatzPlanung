namespace Einsatzplanung.Input.Interfaces;

using System.Collections.Generic;

public interface IConfigService<T> : IConfigService {

	public List<T> ParseSource();

	public void SetData(List<T> data);
}

public interface IConfigService {

	public void SetSource(string path);

}
