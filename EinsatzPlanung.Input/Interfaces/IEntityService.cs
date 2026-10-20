namespace Einsatzplanung.Input.Interfaces;

using System.Collections.Generic;

public interface IEntityService<T> : IEntityService{

	public List<T> GetEntities();

	public T? GetById(int id);

	public bool TryGetById(int id, out T? result);

}

public interface IEntityService {

	public void SetSource(string path);

}
