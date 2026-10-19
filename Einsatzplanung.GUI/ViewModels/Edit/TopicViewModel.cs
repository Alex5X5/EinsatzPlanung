namespace Einsatzplanung.GUI.ViewModels.Edit;

using Avalonia.Media;

using CommunityToolkit.Mvvm.ComponentModel;

using Einsatzplanung.Excel.Interfaces;
using Einsatzplanung.GUI.Interfaces;
using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Input.Services;
using Einsatzplanung.Types.Models;
using Einsatzplanung.Types.Models.Configuration;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

public partial class TopicViewModel : ViewModelBase, IEditViewChild {

	private IConfigExportService<TopicConfig> exportService;
	private IConfigService<TopicConfig> configService;

	public string Header { get; }

	[ObservableProperty]
	private ObservableCollection<TopicCardViewModel> cards;

	[ObservableProperty]
	private int activeCardIndex = -1;

	public TopicViewModel(IConfigService<TopicConfig> configService, IConfigExportService<TopicConfig> exportService) {
		this.exportService = exportService;
		this.configService = configService;
		Cards = [];
	}

	public void AddCard() {
		Cards.Add(new TopicCardViewModel("New Topic", Colors.White));
	}

	protected override async Task OnLoadAsync() {
		List<TopicCardViewModel> cards = await Task.Run(
			() => configService
				.GetData()
				.Select(t => new TopicCardViewModel(t))
				.ToList());
		Cards = new(cards);
	}

	protected override async Task OnUnloadAsync() {
		await SetEntitiesFromStateAsync();
	}

	public async Task ExportCardsAsync(string path) {
		await SetEntitiesFromStateAsync();
		await exportService.ExportAsync(path, configService.GetData());
	}

	public async Task SetEntitiesFromStateAsync() {
		await Task.Run(() => {
			List<TopicConfig> topics = Cards.Select(card => new TopicConfig {
				Name = card.Header,
				Color = card.SelectedColor.ToString()
			}).ToList();

			configService.SetData(topics);
		});
	}
}

// public void AddCard() {
	// 	Cards.Add(new TopicCardViewModel();
	// }
	//
	
	// public async Task ExportCardsAsync(string path) {
	// 	await SetEntitiesFromStateAsync();
	// 	await exportService.ExportAsync(path, configService.GetData());
	// }

	/*protected override async Task OnLoadAsync() {
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
	}*/

	/*protected override async Task OnUnloadAsync() {
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

				DateTime? to = null;

				if (parts.Length == 2) {
					if (!DateTime.TryParseExact(parts[1], "dd.MM.yyyy", null, DateTimeStyles.None, out DateTime to_)) {
						continue;
					} else {
						to = to_;
					}
				}

				var holiday = new HolidayConfig {
					From = DateOnly.FromDateTime(from),
					To = to == null ? null : DateOnly.FromDateTime(to.Value)
				};

				holidays.Add(holiday);
			}

			configService.SetData(holidays);
		});
	}
}*/