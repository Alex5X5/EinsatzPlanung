namespace Einsatzplanung.GUI.ViewModels.Edit;

using CommunityToolkit.Mvvm.Input;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;

using Einsatzplanung.Types.Models;


public partial class TeacherCardViewModel : ViewModelBase {

	public string Header { init; get; }
	public ObservableCollection<TopicCardViewModel> Topics { get; }

	public TeacherCardViewModel(string header, List<Topic> topics) {
		Header = header;
		Topics = new(topics.Select(t => new TopicCardViewModel(t.Name, 0, AddTopicCommand)));
	}

	[RelayCommand]
	private void AddTopic() {
		Topics.Add(new TopicCardViewModel("Neue Spezialisierung", 0, AddTopicCommand));
	}
}