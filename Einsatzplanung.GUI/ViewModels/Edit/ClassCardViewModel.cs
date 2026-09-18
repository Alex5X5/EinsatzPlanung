namespace Einsatzplanung.GUI.ViewModels.Edit;

using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;

using Einsatzplanung.Types.Models.Configuration;

public class ClassCardViewModel : ViewModelBase {
	
	public string Header { get; }
	public ObservableCollection<TeacherTopicCardViewModel> Topics { get; }

	public ClassCardViewModel(string header, List<BlockConfig> topics) {
		Header = header;
		Topics = new(topics.Select(x => new TeacherTopicCardViewModel(x.Name)));
	}
}