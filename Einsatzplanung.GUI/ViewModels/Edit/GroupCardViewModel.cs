namespace Einsatzplanung.GUI.ViewModels.Edit;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models.Configuration;

public partial class GroupCardViewModel : CardViewModel {
	
	[ObservableProperty]
	private string header;

	public ObservableCollection<BlockCardViewModel> Topics { get; }
	
	public GroupConfig Config { init; get; }

	public GroupCardViewModel(GroupConfig group, IRelayCommand<object> removeCommand, int index) : base(removeCommand, index) {
		Config = group;
		Header = group.Name;
		int i = 0;
		var models = group.Blocks.Select(x => new BlockCardViewModel(x, RemoveTopicCommand, i++));
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
		Topics.Add(new BlockCardViewModel(block, RemoveTopicCommand, Topics.Count));
	}

	[RelayCommand]
	private void RemoveTopic(object topic) {
		if (topic is BlockCardViewModel topicToRemove)
			Topics.Remove(topicToRemove);
	}
}