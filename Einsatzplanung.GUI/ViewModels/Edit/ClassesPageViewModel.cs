namespace Einsatzplanung.GUI.ViewModels.Edit;

using System.Collections.ObjectModel;
using System.Linq;

using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models.Configuration;

public class ClassesPageViewModel : ViewModelBase {

	public ObservableCollection<ClassCardViewModel> Cards { get; }

	public ClassesPageViewModel(IConfigService<AgeGroupConfig> configService) {
		var groups = configService.ParseSource().SelectMany(x => x.Groups);
		Cards = new(groups.Select(g => new ClassCardViewModel(g.Name, g.Blocks)));
	}
}