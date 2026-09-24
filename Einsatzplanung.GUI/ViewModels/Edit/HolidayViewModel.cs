namespace Einsatzplanung.GUI.ViewModels.Edit;

using CommunityToolkit.Mvvm.Input;

using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models.Configuration;

using System;
using System.Collections.ObjectModel;
using System.Linq;

public partial class HolidayViewModel {
	public string Header { get; }

	public ObservableCollection<HolidayCardViewModel> Cards { get; }

	public HolidayViewModel(IConfigService<HolidayConfig> configService) {
		var holidays = configService.ParseSource();
		Cards = new(holidays.Select(t => new HolidayCardViewModel($"{t.From:dd.MM.yyyy} - {t.To ?? t.From:dd.MM.yyyy}")));
	}

	[RelayCommand]
	private void AddHoliday() {
		Cards.Add(new HolidayCardViewModel($"{DateTime.Now.Date:dd.MM.yyyy} - {DateTime.Now.Date:dd.MM.yyyy}"));
	}
}