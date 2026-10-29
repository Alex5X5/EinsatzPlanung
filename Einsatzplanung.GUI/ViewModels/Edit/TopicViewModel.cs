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
		var card = new TopicCardViewModel(topic, RemoveCardCommand, Cards.Count);
		Cards.Add(card);
		Groups.Add(new ObservableCollection<TopicCardViewModel>([card]));
	}

	[RelayCommand]
	private void RemoveCard(object card) {
		if (card is TopicCardViewModel topicCard) {
			Cards.Remove(topicCard);
			var group = FindGroup(topicCard);
			group?.Remove(topicCard);
			if (group?.Count == 0)
				Groups.Remove(group);
			UpdateCardIndexes();
		}
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
		Groups.Clear();
		foreach (var card in cards) {
			Cards.Add(card);
			Groups.Add([card]);
		}
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

		UpdateCardIndexes();
	}

	public void MoveCardToNewGroup(TopicCardViewModel card, int targetGroupIndex) {
		var sourceGroup = FindGroup(card);
		if (sourceGroup is null)
			return;

		int sourceGroupIndex = Groups.IndexOf(sourceGroup);
		sourceGroup.Remove(card);
		ObservableCollection<TopicCardViewModel> newGroup;
		bool sourceGroupRemoved = sourceGroup.Count == 0;
		if (sourceGroupRemoved) {
			Groups.Remove(sourceGroup);
			newGroup = sourceGroup;
		} else {
			newGroup = [card];
		}

		if (sourceGroupRemoved && sourceGroupIndex < targetGroupIndex)
			targetGroupIndex--;
		targetGroupIndex = Math.Clamp(targetGroupIndex, 0, Groups.Count);
		Groups.Insert(targetGroupIndex, newGroup);
		UpdateCardIndexes();
	}

	private ObservableCollection<TopicCardViewModel>? FindGroup(TopicCardViewModel card) =>
		Groups.FirstOrDefault(group => group.Contains(card));

	private void UpdateCardIndexes() {
		Cards.Clear();
		int index = 0;
		foreach (var group in Groups) {
			foreach (var card in group) {
				card.Index = index++;
				Cards.Add(card);
			}
		}
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