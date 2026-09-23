namespace Einsatzplanung.GUI.ViewModels.Edit;

using System.Collections.ObjectModel;
using System.Linq;

using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models;

public class ClassesPageViewModel : ViewModelBase {

	public ObservableCollection<ClassCardViewModel> Cards { get; }

	public ClassesPageViewModel(IEntityService<Group> configService) {
		var groups = configService.GetEntities();
		Cards = new(groups.Select(g => new ClassCardViewModel(g.Name, g.Blocks)));
	}
}