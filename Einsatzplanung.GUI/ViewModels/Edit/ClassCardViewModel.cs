namespace Einsatzplanung.GUI.ViewModels.Edit;

using Einsatzplanung.GUI.ViewModels;

using System.Collections.ObjectModel;


public class ClassCardViewModel
{
	public string Header { get; }
	public ObservableCollection<TeacherTopicCardViewModel> Topics { get; }

	public ClassCardViewModel(string header)
	{
		Header = header;
		Topics =
			[
				new TeacherTopicCardViewModel("Grundlagen"),
				new TeacherTopicCardViewModel("Programmierung"),
				new TeacherTopicCardViewModel("Projektmanagement"),
				new TeacherTopicCardViewModel("Netzwerke")
			];
	}
}