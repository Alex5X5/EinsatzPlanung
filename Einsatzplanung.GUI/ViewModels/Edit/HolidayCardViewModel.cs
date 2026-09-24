namespace Einsatzplanung.GUI.ViewModels.Edit;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

public partial class HolidayCardViewModel : ViewModelBase {

	[ObservableProperty]
	private string header;

	public HolidayCardViewModel(string header) {
		Header = header;
	}
}