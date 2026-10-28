namespace Einsatzplanung.GUI.ViewModels.Edit;

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Einsatzplanung.Excel.Interfaces;
using Einsatzplanung.GUI.Interfaces;
using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models.Configuration;

public partial class HolidayViewModel : ViewModelBase, IEditViewChild {

	private IConfigExportService<HolidayConfig> exportService;
	private IConfigService<HolidayConfig> configService;

	public string Header { get; }

	public ObservableCollection<HolidayCardViewModel> Cards { set; get; }

	[ObservableProperty]
	private int activeCardIndex = -1;

	public HolidayViewModel(IConfigService<HolidayConfig> configService, IConfigExportService<HolidayConfig> exportService) {
		this.exportService = exportService;
		this.configService = configService;
		Cards = [];
	}
	
	public void AddCard() {
		string header = $"{DateTime.Now.Date:dd.MM.yyyy} - {DateTime.Now.Date:dd.MM.yyyy}";
		Cards.Add(new HolidayCardViewModel(header, RemoveCardCommand, Cards.Count));
	}

	[RelayCommand]
	private void RemoveCard(object card) {
		if (card is HolidayCardViewModel holidayCard)
			Cards.Remove(holidayCard);
	}

	public async Task ExportCardsAsync(string path, CancellationToken? token = null) {
		await SetEntitiesFromStateAsync();
		var data = await configService.GetDataAsync();
		await exportService.ExportAsync(path, data);
	}

	protected override async Task OnLoadAsync(CancellationToken token) {
		List<HolidayCardViewModel> cards = await Task.Run(
			async () => {
				var holidays = await configService.GetDataAsync(token);
				int i = 0;
				return holidays
					.Select(t => {
						if (t.To == null)
							return new HolidayCardViewModel($"{t.From:dd.MM.yyyy}", RemoveCardCommand, i++);
						else
							return new HolidayCardViewModel($"{t.From:dd.MM.yyyy} - {t.To:dd.MM.yyyy}", RemoveCardCommand, i++);
					})
					.ToList();
			});
		Cards.Clear();
		foreach (var card in cards)
			Cards.Add(card);
	}

	protected override async Task OnUnloadAsync(CancellationToken token) {
		await SetEntitiesFromStateAsync();
	}

	public async Task SetEntitiesFromStateAsync(CancellationToken? token = null) {
		await Task.Run(() => {
			List<HolidayConfig> holidays = new();

			foreach (var card in Cards) {

				string[] parts = card.Header.Split(" - ");

				if (parts.Length == 0)
					continue;
			
				if (!DateTime.TryParseExact(parts[0], "dd.MM.yyyy", null, DateTimeStyles.None, out DateTime from))
					continue;

				DateTime? to = null;

				if (parts.Length == 2) {
					if (!DateTime.TryParseExact(parts[1], "dd.MM.yyyy", null, DateTimeStyles.None, out DateTime to_)) {
						continue;
					} else {
						to = to_;
					}
				}

				var holiday = new HolidayConfig {
					Id = Guid.NewGuid(),
					From = DateOnly.FromDateTime(from),
					To = to == null ? null : DateOnly.FromDateTime(to.Value)
				};

				holidays.Add(holiday);
			}

			configService.SetDataAsync(holidays, token);
		});
	}
}