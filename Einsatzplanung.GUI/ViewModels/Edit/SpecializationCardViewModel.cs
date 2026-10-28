namespace Einsatzplanung.GUI.ViewModels.Edit;

using Avalonia.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Einsatzplanung.GUI.Interfaces;
using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models.Configuration;

using System;
using System.Collections.Generic;
using System.Linq;

public partial class SpecializationCardViewModel : ViewModelBase, IRemovableCardViewModel {

	private IConfigService<TopicConfig> topicService;

	[ObservableProperty]
	private string header;

	public IReadOnlyList<string> TopicNames { get; }

	public IRelayCommand<object> RemoveCommand { init; get; }
	
	public TopicConfig Config { get; private set; }

	public SpecializationCardViewModel(TopicConfig topic, IRelayCommand<object> removeCommand) {
		Config = topic;

		topicService = App.GetRequiredService<IConfigService<TopicConfig>>();

		Header = topic.Name;
		RemoveCommand = removeCommand;

		var topics = topicService.GetDataAsync().Result;
		TopicNames = topics.Select(x=>x.Name).ToList();
	}
}