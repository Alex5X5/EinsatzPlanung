namespace Einsatzplanung.Input.Interfaces;

using System.Collections.Generic;

using Einsatzplanung.Types.Models.Configuration;

public interface IConfigService<T> : IConfigService where T : Config {

	public List<T> GetData();

	public T? GetById(int id);

	public bool TryGetById(int id, out T? result);

	public void SetData(List<T> data);
}

public interface IConfigService {

	public void SetSource(string path);

}
