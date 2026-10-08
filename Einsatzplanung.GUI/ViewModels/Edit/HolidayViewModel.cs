namespace Einsatzplanung.GUI.ViewModels.Edit;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

using Einsatzplanung.Excel.Interfaces;
using Einsatzplanung.GUI.Interfaces;
using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models.Configuration;

public partial class HolidayViewModel : ViewModelBase, IEditViewChild {

	private IConfigExportService<HolidayConfig> exportService;
	private IConfigService<HolidayConfig> configService;

	public string Header { get; }

	public ObservableCollection<HolidayCardViewModel> Cards { get; }

	public HolidayViewModel(IConfigService<HolidayConfig> configService, IConfigExportService<HolidayConfig> exportService) {
		this.exportService = exportService;
		this.configService = configService;
		var holidays = configService.GetData();
		Cards = new(holidays.Select(t => {
			if(t.To == null)
				return new HolidayCardViewModel($"{t.From:dd.MM.yyyy}");
			else
				return new HolidayCardViewModel($"{t.From:dd.MM.yyyy} - {t.To:dd.MM.yyyy}");
		}));
	}
	
	public void AddCard() {
		Cards.Add(new HolidayCardViewModel($"{DateTime.Now.Date:dd.MM.yyyy} - {DateTime.Now.Date:dd.MM.yyyy}"));
	}

	public void ExportCards(string path) {
		SetEntitiesFromState();
		exportService.Export(path, configService.GetData());
	}

	public override void OnUnload() {
		SetEntitiesFromState();
	}


	public void SetEntitiesFromState() {
		List<HolidayConfig> holidays = new();

		foreach (var card in Cards) {

			string[] parts = card.Header.Split(" - ");

			if (parts.Length == 0)
				continue;
			
			if (!DateTime.TryParseExact(parts[0], "dd.MM.yyyy", null, DateTimeStyles.None, out DateTime from))
				continue;

			DateTime to = from;

			if (parts.Length == 2) {
				if (!DateTime.TryParseExact(parts[1], "dd.MM.yyyy", null, DateTimeStyles.None, out DateTime to_)) {
					continue;
				} else {
					to = to_;
				}
			} else {
				to = from;
			}

			var holiday = new HolidayConfig {
				From = DateOnly.FromDateTime(from),
				To = DateOnly.FromDateTime(to)
			};

			holidays.Add(holiday);
		}

		configService.SetData(holidays);
	}
}