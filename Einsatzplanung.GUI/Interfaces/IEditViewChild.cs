using System.Threading;
using System.Threading.Tasks;

namespace Einsatzplanung.GUI.Interfaces; 

public interface IEditViewChild {

	public void AddCard();

	public Task ExportCardsAsync(string path, CancellationToken? token = null);

	public Task SetEntitiesFromStateAsync(CancellationToken? token = null);

}
