namespace Einsatzplanung.GUI.ViewModels.Edit;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Einsatzplanung.Excel.Interfaces;
using Einsatzplanung.GUI.Helpers;
using Einsatzplanung.GUI.Interfaces;
using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models.Configuration;

public partial class TopicViewModel : ViewModelBase, IEditViewChild {

	private IConfigExportService<TopicConfig> exportService;
	private IConfigService<TopicConfig> configService;

	public string Header { get; }

	public ObservableCollection<TopicCardViewModel> Cards { set; get; }

	[ObservableProperty]
	private int activeCardIndex = -1;

	public TopicViewModel(IConfigService<TopicConfig> configService, IConfigExportService<TopicConfig> exportService) {
		this.exportService = exportService;
		this.configService = configService;
		Cards = [];
	}

	public void AddCard() {
		var topic = new TopicConfig() {
			Id = Guid.NewGuid(),
			Name = "",
			Color = "#FFFFFF"
		};
		Cards.Add(new TopicCardViewModel(topic, RemoveCardCommand, Cards.Count));
	}

	[RelayCommand]
	private void RemoveCard(object card) {
		if (card is TopicCardViewModel topicCard)
			Cards.Remove(topicCard);
	}

	protected override async Task OnLoadAsync(CancellationToken token) {
		List<TopicCardViewModel> cards = await Task.Run(async () => {
			var topics = await configService.GetDataAsync(token);
			int i = 0;
			return topics
				.Select(g => new TopicCardViewModel(g, RemoveCardCommand, i++))
				.ToList();
		});
		
		Cards.Clear();
		foreach (var card in cards)
			Cards.Add(card);
	}

	protected override async Task OnUnloadAsync(CancellationToken token) {
		await SetEntitiesFromStateAsync();
	}

	public async Task ExportCardsAsync(string path, CancellationToken? token = null) {
		await SetEntitiesFromStateAsync(token);
		var data = await configService.GetDataAsync(token);
		await exportService.ExportAsync(path, data);
	}

	public async Task SetEntitiesFromStateAsync(CancellationToken? token = null) {
		await Task.Run(async () => {
			List<TopicConfig> topics = [];
			foreach (var card in Cards) {
				var topic = card.Topic;
				topic.Name = card.Header;
				topic.Color = card.SelectedColor.ToRgbHex();
				topics.Add(topic);
			}
			await configService.SetDataAsync(topics, token);
		});
	}
}