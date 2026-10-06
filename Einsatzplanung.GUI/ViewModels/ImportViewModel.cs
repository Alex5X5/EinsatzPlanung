namespace Einsatzplanung.GUI.ViewModels;

using System;
using System.Collections.ObjectModel;
using System.Globalization;

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;

using Microsoft.Extensions.DependencyInjection;

using Einsatzplanung.GUI;
using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Util.Services;
using Einsatzplanung.Util.Interfaces;
using Einsatzplanung.Types.Models;

public partial class ImportViewModel : ViewModelBase {

	private const string YEAR_START_KEY = "ImportViewModel.YearStart";
	private const string YEAR_END_KEY = "ImportViewModel.YearEnd";

	private GeneralConfigService configService;
	private ILastSelectionService selectionService;

	public ObservableCollection<ImportCardViewModel> Cards { get; }

	[ObservableProperty]
	private DateTime yearStartDate;

	partial void OnYearStartDateChanged(DateTime value) {
		configService.YearStartDate = value;
		selectionService.SetSelection(YEAR_START_KEY, value.ToString("dd.MM.yyyy"));
	}

	[ObservableProperty]
	private DateTime yearEndDate;

	partial void OnYearEndDateChanged(DateTime value) {
		configService.YearEndDate = value;
		selectionService.SetSelection(YEAR_END_KEY, value.ToString("dd.MM.yyyy"));
	}

	public ImportViewModel(GeneralConfigService configService, ILastSelectionService selectionService) : base() {
		
		this.configService = configService;
		this.selectionService = selectionService;

		if (DateTime.TryParseExact(
			selectionService.GetSelection(YEAR_START_KEY) ?? "",
			"dd.MM.yyyy",
			CultureInfo.InvariantCulture,
			DateTimeStyles.None,
			out var start)) {
			YearStartDate = start;
		} else {
			YearStartDate = new(2026, 1, 1);
		}

		if (DateTime.TryParseExact(
			selectionService.GetSelection(YEAR_END_KEY) ?? "",
			"dd.MM.yyyy",
			CultureInfo.InvariantCulture,
			DateTimeStyles.None,
			out var stop)) {
			YearEndDate = stop;
		} else {
			YearEndDate = new(2027, 1, 1);
		}

		var services = App.Current.Services;

		Cards = [
			new("Ausbilder & Spezialisierungen", services.GetRequiredService<IEntityService<Teacher>>(), selectionService),
			new("Ausbildungsgruppen & Themen", services.GetRequiredService<IEntityService<AgeGroup>>(), selectionService),
			new("Urlaub & Feiertage", services.GetRequiredService<IEntityService<Holiday>>(), selectionService)
			//new("Ausbildungsinhalte", services.GetRequiredService<IEntityService<Topic>>())
			//new("Praktikumszeiten", services.GetRequiredService<IEntityService<Teacher>>()),
			//new("Schulwochen", services.GetRequiredService<IEntityService<Teacher>>())
		];
	}

	[RelayCommand]
	private void OnGoToNextPage() {
		App.Current.Services.GetRequiredService<ILastSelectionService>().SaveChanges();
		App.Current.Services.GetRequiredService<MainViewModel>().ChangePage<EditViewModel>();
		//var plan = App.Current.Services.GetRequiredService<IGeneratorService>().GeneratePlan();
	}
}
