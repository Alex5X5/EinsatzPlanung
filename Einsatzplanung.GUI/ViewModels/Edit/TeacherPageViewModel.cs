using System.Collections.ObjectModel;

namespace Einsatzplanung.GUI.ViewModels.Edit {
	public class TeacherPageViewModel {
		public string Header { get; }

		public ObservableCollection<TeacherCardViewModel> Cards { get; }

		public TeacherPageViewModel()
		{
			Cards =
			[
				new TeacherCardViewModel("Herr Schule"),
				new TeacherCardViewModel("Herr Bla"),
				new TeacherCardViewModel("Herr Schwank"),
				new TeacherCardViewModel("Herr Lehnert")
			];
		}
	}
}