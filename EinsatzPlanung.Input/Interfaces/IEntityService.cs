namespace EinsatzPlanung.Input.Interfaces;

using System.Collections.Generic;

public interface IEntityService<T> {

	public void SetSource(string path);

	public List<T> ParseSource();

}
