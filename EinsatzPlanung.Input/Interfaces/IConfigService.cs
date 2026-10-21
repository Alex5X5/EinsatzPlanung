namespace Einsatzplanung.Input.Interfaces;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Einsatzplanung.Types.Models.Configuration;

public interface IConfigService<T> : IConfigService where T : Config {

	public List<T> GetData();

	public T? GetById(Guid id);

	public bool TryGetById(Guid id, out T? result);

	public void SetData(List<T> data);
}

public interface IConfigService {

	public void SetSource(string path);

	public Task UpdateReferencesAsync();

}
