namespace Einsatzplanung.GUI.ViewModels.Edit;

using CommunityToolkit.Mvvm.Input;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;

using Einsatzplanung.Types.Models;
using Einsatzplanung.GUI.Interfaces;

public partial class TeacherCardViewModel : ViewModelBase {

	public string Header { get; }
	public ObservableCollection<TopicCardViewModel> Topics { get; }

	public TeacherCardViewModel(string header, List<Topic> topics) {
		Header = header;
		Topics = new(topics.Select(t => new TopicCardViewModel(t.Name, 0, RemoveTopicCommand)));
	}

	[RelayCommand]
	private void AddTopic() {
		Topics.Add(new TopicCardViewModel("Neue Spezialisierung", 0, RemoveTopicCommand));
	}

	[RelayCommand]
	private void RemoveTopic(object topic) {
		if(topic is IRemovableCardViewModel model)
			if(Topics.FirstOrDefault(t => t.Header == model.Header) is TopicCardViewModel topicToRemove)
				Topics.Remove(topicToRemove);
	}
}