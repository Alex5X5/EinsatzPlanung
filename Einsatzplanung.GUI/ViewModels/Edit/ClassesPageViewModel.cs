namespace Einsatzplanung.GUI.ViewModels.Edit;

using CommunityToolkit.Mvvm.Input;
using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models;

using System.Collections.ObjectModel;
using System.Linq;

public partial class ClassesPageViewModel : ViewModelBase {

	public ObservableCollection<ClassCardViewModel> Cards { get; }

	public ClassesPageViewModel(IEntityService<Group> configService) {
		var groups = configService.GetEntities();
		Cards = new(groups.Select(g => new ClassCardViewModel(g.Name, g.Blocks)));
	}

	[RelayCommand]
	private void AddClass() {
		Cards.Add(new ClassCardViewModel("Neue Klasse", []));
	}
}