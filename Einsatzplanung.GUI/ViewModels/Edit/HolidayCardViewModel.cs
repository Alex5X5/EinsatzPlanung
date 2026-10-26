namespace Einsatzplanung.GUI.ViewModels.Edit;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

public partial class HolidayCardViewModel : CardViewModel {

	public HolidayCardViewModel(string header, IRelayCommand<object> removeCommand, int index) : base(removeCommand, header, index) {
		Header = header;
	}
}