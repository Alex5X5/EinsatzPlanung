namespace Einsatzplanung.GUI.ViewModels;

using System;
using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;

using Microsoft.Extensions.DependencyInjection;

using Einsatzplanung.GUI;
using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models.Configuration;
using Einsatzplanung.Util.Services;

public partial class ImportViewModel : ViewModelBase {

	private GeneralConfigService configService;

	public ObservableCollection<ImportCardViewModel> Cards { get; }

	[ObservableProperty]
	private DateTime yearStartDate;

	partial void OnYearStartDateChanged(DateTime value) {
		configService.YearStartDate = value;
	}

	[ObservableProperty]
	private DateTime yearEndDate;

	partial void OnYearEndDateChanged(DateTime value) {
		configService.YearEndDate = value;
	}

	public ImportViewModel(GeneralConfigService configService) : base() {
		
		this.configService = configService;

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
