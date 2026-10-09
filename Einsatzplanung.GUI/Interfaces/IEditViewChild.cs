using System.Threading.Tasks;

namespace Einsatzplanung.GUI.Interfaces; 

public interface IEditViewChild {

	public void AddCard();

	public Task ExportCardsAsync(string path);

	public Task SetEntitiesFromStateAsync();

}
