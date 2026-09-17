using CommunityToolkit.Mvvm.Input;

namespace Einsatzplanung.GUI.ViewModels;

using Microsoft.Extensions.DependencyInjection;

using System.Collections.ObjectModel;

using Einsatzplanung.GUI;
using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models.Configuration;

public partial class ImportViewModel : ViewModelBase {
	
	public ObservableCollection<ImportCardViewModel> Cards { get; }

	[ObservableProperty]
	private DateTime yearStartDate;

	[ObservableProperty]
	private DateTime yearEndDate;

	public ImportViewModel() : base() {
		var services = App.Current.Services;
	
		Cards = [
			new("Ausbilder & Spezialisierungen", services.GetRequiredService<IConfigService<TeacherConfig>>()),
			new("Ausbildungsgruppen & Themen", services.GetRequiredService<IConfigService<AgeGroupConfig>>())
			//new("Ausbildungsinhalte", services.GetRequiredService<IEntityService<Topic>>())
			//new("Urlaubswochen & Feiertage", services.GetRequiredService<IEntityService<Block>>()),
			//new("Praktikumszeiten", services.GetRequiredService<IEntityService<Teacher>>()),
			//new("Schulwochen", services.GetRequiredService<IEntityService<Teacher>>())
		];
	}

	[RelayCommand]
	private void OnGoToNextPage() {
		App.Current.Services.GetRequiredService<MainViewModel>().ChangePage<EditViewModel>();
		//var plan = App.Current.Services.GetRequiredService<IGeneratorService>().GeneratePlan();
	}
}
