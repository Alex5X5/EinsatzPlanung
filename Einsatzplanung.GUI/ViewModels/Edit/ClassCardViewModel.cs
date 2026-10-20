namespace Einsatzplanung.GUI.ViewModels.Edit;

using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

using Avalonia.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Einsatzplanung.GUI.Interfaces;
using Einsatzplanung.Types.Models.Configuration;

public partial class ClassCardViewModel : CardViewModel {
	
	[ObservableProperty]
	private string header;
	public ObservableCollection<BlockCardViewModel> Topics { get; }
	public GroupConfig Config { init; get; }
	private readonly IReadOnlyList<string> topicNames;

	public ClassCardViewModel(GroupConfig group, int index, IReadOnlyList<string> topicNames) : base(index) {
		this.topicNames = topicNames;
		Config = group;
		Header = group.Name;
		var models = group.Blocks.Select(
			block => new BlockCardViewModel(block.Topic, block.Count, topicNames, RemoveTopicCommand));
		Topics = new(models);
	}

	[RelayCommand]
	private void AddTopic() {
		if (topicNames.Count > 0)
			Topics.Add(new BlockCardViewModel(topicNames[0], 1, topicNames, RemoveTopicCommand));
	}

	[RelayCommand]
	private void RemoveTopic(object topic) {
		if (topic is BlockCardViewModel topicToRemove)
			Topics.Remove(topicToRemove);
	}
}