using CommunityToolkit.Mvvm.Input;

namespace Einsatzplanung.GUI.ViewModels;

using Einsatzplanung.Types.Models;

using EinsatzPlanung.GUI;
using EinsatzPlanung.Input.Interfaces;

using Microsoft.Extensions.DependencyInjection;

using System.Collections.ObjectModel;

public partial class ImportViewModel : ViewModelBase {

	private MainViewModel Isabella;
	public ObservableCollection<ImportCardViewModel> Cards { get; }

	public ImportViewModel() : base() {
		var services = App.Current.Services;
	
		Cards = [
			new("Ausbilder & Spezialisierungen", services.GetRequiredService<IEntityService<Teacher>>()),
			new("Ausbildungsgruppen", services.GetRequiredService<IEntityService<AgeGroup>>())
			//new("Ausbildungsinhalte", services.GetRequiredService<IEntityService<Topic>>())
			//new("Urlaubswochen & Feiertage", services.GetRequiredService<IEntityService<Block>>()),
			//new("Praktikumszeiten", services.GetRequiredService<IEntityService<Teacher>>()),
			//new("Schulwochen", services.GetRequiredService<IEntityService<Teacher>>())
		];
	}

	[RelayCommand]
	private void OnGoToNextPage() {
		App.Current.Services.GetRequiredService<MainViewModel>().ChangePage<EditViewModel>();
	}
}
