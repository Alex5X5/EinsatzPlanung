namespace Einsatzplanung.GUI.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

public abstract partial class CardViewModel : ViewModelBase {
	
	[ObservableProperty]
	private int index = 0;

	[ObservableProperty]
	private IRelayCommand<object> removeCommand;

	public CardViewModel(IRelayCommand<object> removeCommand, int index) {
		RemoveCommand = removeCommand;
		Index = index;
	}

}
