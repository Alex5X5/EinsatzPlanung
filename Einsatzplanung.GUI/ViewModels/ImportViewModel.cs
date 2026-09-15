namespace Einsatzplanung.GUI.ViewModels;

using CommunityToolkit.Mvvm.Input;

using Microsoft.Extensions.DependencyInjection;

using System.Collections.ObjectModel;

using Einsatzplanung.GUI;
using Einsatzplanung.Types.Models;
using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Generation.Interfaces;

public partial class ImportViewModel : ViewModelBase {
	
	public ObservableCollection<ImportCardViewModel> Cards { get; }

	public ImportViewModel() : base() {
		var services = App.Current.Services;
	
		Cards = [
			new("Ausbilder & Spezialisierungen", services.GetRequiredService<IEntityService<Teacher>>()),
			new("Ausbildungsgruppen & Themen", services.GetRequiredService<IEntityService<AgeGroup>>())
			//new("Ausbildungsinhalte", services.GetRequiredService<IEntityService<Topic>>())
			//new("Urlaubswochen & Feiertage", services.GetRequiredService<IEntityService<Block>>()),
			//new("Praktikumszeiten", services.GetRequiredService<IEntityService<Teacher>>()),
			//new("Schulwochen", services.GetRequiredService<IEntityService<Teacher>>())
		];
	}

	[RelayCommand]
	private void OnGoToNextPage() {
		//App.Current.Services.GetRequiredService<MainViewModel>().ChangePage<EditViewModel>();
		App.Current.Services.GetRequiredService<IGeneratorService>().GeneratePlan();
	}
}
