namespace Einsatzplanung.GUI.ViewModels.Edit;

using System.Linq;
using System.Collections.ObjectModel;

using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models.Configuration;

public class TeacherPageViewModel {
	public string Header { get; }

	public ObservableCollection<TeacherCardViewModel> Cards { get; }

	public TeacherPageViewModel(IConfigService<TeacherConfig> configService) {
		var teachers = configService.ParseSource();
		Cards = new(teachers.Select(t => new TeacherCardViewModel(t.Name, t.Specializations)));
	}
}