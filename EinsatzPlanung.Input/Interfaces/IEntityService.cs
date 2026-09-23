namespace Einsatzplanung.Input.Interfaces;

using System.Collections.Generic;

public interface IEntityService<T> : IEntityService{

	public List<T> GetEntities();

}

public interface IEntityService {

	public void SetSource(string path);

}
