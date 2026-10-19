namespace Einsatzplanung.GUI.ViewModels.Edit;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Einsatzplanung.GUI.Interfaces;
using Einsatzplanung.Types.Models.Configuration;

using System.Collections.ObjectModel;
using System.Linq;

public partial class ClassCardViewModel : CardViewModel {
	
	[ObservableProperty]
	private string header;
	public ObservableCollection<TopicCardViewModel> Topics { get; }
	public GroupConfig Config { init; get; }

	public ClassCardViewModel(GroupConfig group, int index) : base(index) {
		Config = group;
		Header = group.Name;
		var models = group.Blocks.Select(x => new TopicCardViewModel(x.Topic, x.Count, RemoveTopicCommand));
		Topics = new(models);
	}

	[RelayCommand]
	private void AddTopic() {
		Topics.Add(new TopicCardViewModel("Neues Thema", 1, RemoveTopicCommand));
	}

	[RelayCommand]
	private void RemoveTopic(object topic) {
		if (topic is IRemovableCardViewModel model)
			if (Topics.FirstOrDefault(t => t.Header == model.Header) is TopicCardViewModel topicToRemove)
				Topics.Remove(topicToRemove);
	}
}