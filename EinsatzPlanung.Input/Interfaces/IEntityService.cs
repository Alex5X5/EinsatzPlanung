namespace Einsatzplanung.Input.Interfaces;

using System.Collections.Generic;

public interface IEntityService<T> {

	public List<T> GetEntities();

}
