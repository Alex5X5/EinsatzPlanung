namespace Einsatzplanung.GUI.ViewModels.Edit;

using CommunityToolkit.Mvvm.ComponentModel;

public partial class HolidayCardViewModel : ViewModelBase {

	[ObservableProperty]
	private string header;

	public HolidayCardViewModel(string header) {
		Header = header;
	}
}