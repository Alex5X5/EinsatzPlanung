namespace Einsatzplanung.GUI.ViewModels.Edit;

using System.Linq;
using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Microsoft.Extensions.DependencyInjection;

using Einsatzplanung.GUI.Interfaces;
using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models.Configuration;

public partial class ClassCardViewModel : CardViewModel {
	
	[ObservableProperty]
	private string header;

	public ObservableCollection<BlockCardViewModel> Topics { get; }
	
	public GroupConfig Config { init; get; }

	public ClassCardViewModel(GroupConfig group, int index) : base(index) {
		Config = group;
		Header = group.Name;
		var models = group.Blocks.Select(x => new BlockCardViewModel(x, RemoveTopicCommand));
		Topics = new(models);
	}

	[RelayCommand]
	private void AddTopic() {
		var topic = App.Current.Services.GetRequiredService<IConfigService<TopicConfig>>().GetData().FirstOrDefault(TopicConfig.Default);
		var block = new BlockConfig() {
			Id = 0,
			Count = 1,
			Topic = topic
		};
		Topics.Add(new BlockCardViewModel(block, RemoveTopicCommand));
	}

	[RelayCommand]
	private void RemoveTopic(object topic) {
		if (topic is IRemovableCardViewModel model)
			if (Topics.FirstOrDefault(t => t.Header == model.Header) is BlockCardViewModel topicToRemove)
				Topics.Remove(topicToRemove);
	}
}