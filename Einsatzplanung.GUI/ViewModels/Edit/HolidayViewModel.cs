namespace Einsatzplanung.GUI.ViewModels.Edit;

using System.Linq;
using System.Collections.ObjectModel;

using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models.Configuration;

public class HolidayViewModel {
	public string Header { get; }

	public ObservableCollection<HolidayCardViewModel> Cards { get; }

	public HolidayViewModel(IConfigService<HolidayConfig> configService) {
		var holidays = configService.ParseSource();
		Cards = new(holidays.Select(t => new HolidayCardViewModel($"{t.From:dd.MM.yyyy} - {t.To ?? t.From:dd.MM.yyyy}")));
	}
}