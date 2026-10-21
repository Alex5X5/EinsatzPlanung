namespace Einsatzplanung.GUI.ViewModels.Edit;

using System;
using System.Collections.Generic;
using System.Linq;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Einsatzplanung.GUI.Interfaces;
using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models.Configuration;

public partial class BlockCardViewModel : ViewModelBase, IRemovableCardViewModel {

	private IConfigService<TopicConfig> topicService;

	[ObservableProperty]
	private string header;

	partial void OnHeaderChanged(string value) {
		Config = new BlockConfig() {
			Id = Guid.NewGuid(),
			Topic = topicService.GetByName(value) ?? TopicConfig.Default,
			Count = Config.Count
		};
	}

	[ObservableProperty]
	private string count;

	public IReadOnlyList<string> TopicNames { get; }

	public IRelayCommand<object> RemoveCommand { init; get; }
	
	public BlockConfig Config { get; private set; }

	public BlockCardViewModel(BlockConfig block, IRelayCommand<object> removeCommand) {
		Config = block;

		topicService = App.GetRequiredService<IConfigService<TopicConfig>>();

		Header = Config.Topic.Name;
		Count = Config.Count.ToString();

		RemoveCommand = removeCommand;

		var topics = topicService.GetData();
		TopicNames = topics.Select(x => x.Name).ToList();
	}
}