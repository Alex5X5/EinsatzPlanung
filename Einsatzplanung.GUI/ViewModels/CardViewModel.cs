namespace Einsatzplanung.GUI.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Einsatzplanung.GUI.Interfaces;

public abstract partial class CardViewModel : ViewModelBase, IRemovableCardViewModel {

	[ObservableProperty]
	private string header = "";

	[ObservableProperty]
	private int index = 0;

	[ObservableProperty]
	private IRelayCommand<object> removeCommand;

	public CardViewModel(IRelayCommand<object> removeCommand, int index) {
		RemoveCommand = removeCommand;
		Index = index;
	}

}
