namespace Einsatzplanung.GUI.ViewModels.Edit;

public class HolidayCardViewModel : ViewModelBase {

	public string Header { get; }

	public HolidayCardViewModel(string header) {
		Header = header;
	}
}