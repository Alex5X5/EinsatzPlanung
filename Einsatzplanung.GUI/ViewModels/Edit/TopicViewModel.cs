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
using Einsatzplanung.GUI.Controls;
using Einsatzplanung.GUI.Helpers;
using Einsatzplanung.GUI.Interfaces;
using Einsatzplanung.GUI.Views.Edit;
using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models.Configuration;

public partial class TopicViewModel : ViewModelBase, IEditViewChild {

	private IConfigExportService<TopicConfig> exportService;
	private IConfigService<TopicConfig> configService;

	public string Header { get; }

	public ObservableCollection<TopicCardViewModel> Cards { set; get; }
	public ObservableCollection<ObservableCollection<TopicCardViewModel>> Groups { get; } = [];

	[ObservableProperty]
	private int activeCardIndex = -1;

	public bool ShowLevelView { get; } = false;

	public bool ShowGroupsView { get; } = true;

	public TopicViewModel(IConfigService<TopicConfig> configService, IConfigExportService<TopicConfig> exportService) {
		this.exportService = exportService;
		this.configService = configService;
		Cards = [];
	}

	public void AddCard() {
		var topic = new TopicConfig() {
			Id = Guid.NewGuid(),
			Name = "",
			Color = "#FFFFFF",
			Level = Convert.ToString(Cards.Count)
		};
		ObservableCollection<TopicCardViewModel> col = [];
		col.Add(new TopicCardViewModel(topic, RemoveCardCommand, Cards.Count));
		Groups.Add(col);
	}

	[RelayCommand]
	private void RemoveCard(object card) {
		if (card is TopicCardViewModel topicCard)
			foreach (var row in Groups)
				if (row.Remove(topicCard))
					break;
	}

	protected override async Task OnLoadAsync(CancellationToken token) {
		List<List<TopicCardViewModel>> cards = await Task.Run(async () => {
			var topics = await configService.GetDataAsync(token);
			int i = 0;
			var grouped = topics
				.GroupBy(g => g.Level)
				.OrderBy(x => int.TryParse(x.Key, out int level) ? level : 1);
			return grouped
				.Select(g => g.Select(g => new TopicCardViewModel(g, RemoveCardCommand, i++)).ToList())
				.ToList();
		});

		Groups.Clear();
		foreach (var row in cards) {
			ObservableCollection<TopicCardViewModel> newRow = [];
			foreach (var card in row)
				newRow.Add(card);
			Groups.Add(newRow);
		}
	}

	[RelayCommand]
	private void OnCardMoved(ItemMovedEventArgs args) {
		if (args.Item is not TopicCardViewModel card)
			return;
		card.Topic.Level = Convert.ToString(args.NewRow);
	}

	[RelayCommand]
	public void MoveCardToGroup(CardMovedEventArgs args) {
		if(args.Model is not TopicCardViewModel card)
			return;
		
		var sourceGroup = FindGroup(card);
		if (sourceGroup is null)
			return;

		if (sourceGroup == args.TargetGroup) {
			int currentIndex = sourceGroup.IndexOf(card);
			if (currentIndex < 0)
				return;

			var targetCardIndex = Math.Clamp(args.TargetCardIndex, 0, sourceGroup.Count - 1);
			if (currentIndex != targetCardIndex)
				sourceGroup.Move(currentIndex, targetCardIndex);
		} else {
			sourceGroup.Remove(card);
			if (sourceGroup.Count == 0)
				Groups.Remove(sourceGroup);

			var targetCardIndex = Math.Clamp(args.TargetCardIndex, 0, args.TargetGroup.Count);
			args.TargetGroup.Insert(targetCardIndex, card);
		}
	}

	private ObservableCollection<TopicCardViewModel>? FindGroup(TopicCardViewModel card) =>
		Groups.FirstOrDefault(group => group.Contains(card));
	
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
			int levelIndex = 1;
			foreach (var level in Groups) {
				foreach (var card in level) {
					var topic = new TopicConfig() {
						Id = card.Topic.Id,
						Name = card.Header,
						Level = levelIndex.ToString(),
						Color = card.SelectedColor.ToRgbHex()
					};
					topics.Add(topic);
				}
				levelIndex++;
			}
			await configService.SetDataAsync(topics, token);
		});
	}
}