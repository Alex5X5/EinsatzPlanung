namespace Einsatzplanung.GUI.ViewModels.Edit;

using CommunityToolkit.Mvvm.Input;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;

using Einsatzplanung.Types.Models;


public partial class TeacherCardViewModel : ViewModelBase {

	public string Header { get; }
	public ObservableCollection<TeacherTopicCardViewModel> Topics { get; }

	public TeacherCardViewModel(string header, List<Topic> topics) {
		Header = header;
		Topics = new(topics.Select(t => new TeacherTopicCardViewModel(t.Name)));
	}

	[RelayCommand]
	private void AddTopic() {
		Topics.Add(new TeacherTopicCardViewModel("Neue Spezialisierung"));
	}
}