namespace Einsatzplanung.GUI.ViewModels.Edit;

using System.Linq;
using System.Collections.ObjectModel;

using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models;

public class TeacherPageViewModel {
	public string Header { get; }

	public ObservableCollection<TeacherCardViewModel> Cards { get; }

	public TeacherPageViewModel(IEntityService<Teacher> configService) {
		var teachers = configService.GetEntities();
		Cards = new(teachers.Select(t => new TeacherCardViewModel(t.Name, t.Specializations)));
	}
}