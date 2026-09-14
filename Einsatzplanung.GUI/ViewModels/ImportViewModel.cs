namespace Einsatzplanung.GUI.ViewModels;

using Einsatzplanung.Types.Models;

using EinsatzPlanung.GUI;
using EinsatzPlanung.Input.Interfaces;

using Microsoft.Extensions.DependencyInjection;

using System.Collections.ObjectModel;

public class ImportViewModel : ViewModelBase {

	public ObservableCollection<ImportCardViewModel> Cards { get; }

	public ImportViewModel() : base() {

		var services = App.Current.Services;
	
		Cards = [
			new("Ausbilder & Spezialisierungen", services.GetRequiredService<IEntityService<Teacher>>()),
			new("Ausbildungsgruppe", services.GetRequiredService<IEntityService<Group>>()),
			new("Ausbildungsinhalte", services.GetRequiredService<IEntityService<Topic>>()),
			new("Urlaubswochen & Feiertage", services.GetRequiredService<IEntityService<Block>>()),
			new("Praktikumszeiten", services.GetRequiredService<IEntityService<Teacher>>()),
			new("Schulwochen", services.GetRequiredService<IEntityService<Teacher>>())
		];
	}
}
