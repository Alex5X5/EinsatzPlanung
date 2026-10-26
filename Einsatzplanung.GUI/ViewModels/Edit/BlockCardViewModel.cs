namespace Einsatzplanung.GUI.ViewModels.Edit;

using System;
using System.Collections.Generic;
using System.Linq;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Einsatzplanung.GUI.Interfaces;
using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models.Configuration;

public partial class BlockCardViewModel : CardViewModel, IRemovableCardViewModel {

	private IConfigService<TopicConfig> topicService;

	[ObservableProperty]
	private string count;

	public IReadOnlyList<string> TopicNames { get; }
	
	public BlockConfig Config { get; private set; }

	public BlockCardViewModel(BlockConfig block, IRelayCommand<object> removeCommand, int index) : base(removeCommand, block.Topic.Name, index) {
		Config = block;

		topicService = App.GetRequiredService<IConfigService<TopicConfig>>();

		Count = Config.Count.ToString();

		var topics = topicService.GetData();
		TopicNames = topics.Select(x => x.Name).ToList();
	}
}