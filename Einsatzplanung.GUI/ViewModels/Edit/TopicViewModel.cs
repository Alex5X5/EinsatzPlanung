namespace Einsatzplanung.GUI.ViewModels.Edit;

using System.Threading.Tasks;
using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;

using Einsatzplanung.GUI.Interfaces;

public partial class TopicViewModel : ViewModelBase, IEditViewChild { // IEditViewChild  missing for skeleton

	public string Header { get; }

	[ObservableProperty]
	private ObservableCollection<TopicCardViewModel> cards;

	[ObservableProperty]
	private int activeCardIndex = -1;

	public TopicViewModel() {
		Cards = [];
	}

	public void AddCard() {
		
	}

	public Task ExportCardsAsync(string path) {
		return Task.CompletedTask;
	}

	public Task SetEntitiesFromStateAsync() {
		return Task.CompletedTask;
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