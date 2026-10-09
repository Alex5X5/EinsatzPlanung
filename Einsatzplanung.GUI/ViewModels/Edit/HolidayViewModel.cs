namespace Einsatzplanung.GUI.ViewModels.Edit;

using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

using CommunityToolkit.Mvvm.ComponentModel;

using Einsatzplanung.Excel.Interfaces;
using Einsatzplanung.GUI.Interfaces;
using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models.Configuration;

public partial class HolidayViewModel : ViewModelBase, IEditViewChild {

	private IConfigExportService<HolidayConfig> exportService;
	private IConfigService<HolidayConfig> configService;

	public string Header { get; }

	[ObservableProperty]
	private ObservableCollection<HolidayCardViewModel> cards;

	[ObservableProperty]
	private int activeCardIndex = -1;

	public HolidayViewModel(IConfigService<HolidayConfig> configService, IConfigExportService<HolidayConfig> exportService) {
		this.exportService = exportService;
		this.configService = configService;
		Cards = [];
	}
	
	public void AddCard() {
		Cards.Add(new HolidayCardViewModel($"{DateTime.Now.Date:dd.MM.yyyy} - {DateTime.Now.Date:dd.MM.yyyy}", Cards.Count));
	}

	public async Task ExportCardsAsync(string path) {
		await SetEntitiesFromStateAsync();
		await exportService.ExportAsync(path, configService.GetData());
	}

	protected override async Task OnLoadAsync() {
		List<HolidayCardViewModel> cards = await Task.Run(
			() => {
				var holidays = configService.GetData();
				int i = 0;
				return holidays
					.Select(t => {
						if (t.To == null)
							return new HolidayCardViewModel($"{t.From:dd.MM.yyyy}", i++);
						else
							return new HolidayCardViewModel($"{t.From:dd.MM.yyyy} - {t.To:dd.MM.yyyy}", i++);
					})
					.ToList();
			});
		Cards = new(cards);
	}

	protected override async Task OnUnloadAsync() {
		await SetEntitiesFromStateAsync();
	}

	public async Task SetEntitiesFromStateAsync() {
		await Task.Run(() => {
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
		});
	}
}