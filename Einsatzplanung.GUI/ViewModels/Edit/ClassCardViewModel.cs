namespace Einsatzplanung.GUI.ViewModels.Edit;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models.Configuration;

public partial class ClassCardViewModel : CardViewModel {
	
	[ObservableProperty]
	private string header;

	public ObservableCollection<BlockCardViewModel> Topics { get; }
	
	public GroupConfig Config { init; get; }

	public ClassCardViewModel(GroupConfig group, IRelayCommand<object> removeCommand, int index) : base(removeCommand, index) {
		Config = group;
		Header = group.Name;
		var models = group.Blocks.Select(x => new BlockCardViewModel(x, RemoveTopicCommand));
		Topics = new(models);
	}

	[RelayCommand]
	private void AddTopic() {
		var topic = App.GetRequiredService<IConfigService<TopicConfig>>().GetData().FirstOrDefault(TopicConfig.Default);
		var block = new BlockConfig() {
			Id = Guid.NewGuid(),
			Count = 1,
			Topic = topic
		};
		Topics.Add(new BlockCardViewModel(block, RemoveTopicCommand));
	}

	[RelayCommand]
	private void RemoveTopic(object topic) {
		if (topic is BlockCardViewModel topicToRemove)
			Topics.Remove(topicToRemove);
	}
}