namespace Einsatzplanung.GUI.ViewModels.Edit;

using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;

using Einsatzplanung.Types.Models;


public class TeacherCardViewModel : ViewModelBase {

	public string Header { get; }
	public ObservableCollection<TeacherTopicCardViewModel> Topics { get; }

	public TeacherCardViewModel(string header, List<Topic> topics) {
		Header = header;
		Topics = new(topics.Select(t => new TeacherTopicCardViewModel(t.Name)));
	}
}