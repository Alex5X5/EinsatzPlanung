namespace Einsatzplanung.GUI.ViewModels;

using System.Collections.ObjectModel;

public class ImportViewModel : ViewModelBase {

	public ObservableCollection<ImportCardViewModel> Cards { get; }

	public ImportViewModel() : base() {
		Cards = [
			new("Ausbilder & Spezialisierungen"),
			new("Ausbildungsgruppe"),
			new("Ausbildungsinhalte"),
			new("Urlaubswochen & Feiertage"),
			new("Praktikumszeiten"),
			new("Schulwochen")
		];
	}
}
