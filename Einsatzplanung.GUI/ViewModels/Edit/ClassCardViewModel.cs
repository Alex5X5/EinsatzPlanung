namespace Einsatzplanung.GUI.ViewModels.Edit;

using CommunityToolkit.Mvvm.Input;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;

using Einsatzplanung.Types.Models;
using Einsatzplanung.GUI.Interfaces;

public partial class ClassCardViewModel : ViewModelBase {
	
	public string Header { get; }
	public ObservableCollection<TopicCardViewModel> Topics { get; }

	public ClassCardViewModel(string header, List<Block> topics) {
		Header = header;
		Topics = new(topics.Select(x => new TopicCardViewModel(x.Name, x.Count, RemoveTopicCommand)));
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