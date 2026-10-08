namespace Einsatzplanung.GUI.ViewModels.Edit;

using CommunityToolkit.Mvvm.ComponentModel;

public partial class HolidayCardViewModel : CardViewModel {

	[ObservableProperty]
	private string header;

	public HolidayCardViewModel(string header, int index) : base(index) {
		Header = header;
	}
}