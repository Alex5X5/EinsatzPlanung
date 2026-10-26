namespace Einsatzplanung.GUI.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Einsatzplanung.GUI.Interfaces;

public abstract partial class CardViewModel : ViewModelBase, IRemovableCardViewModel {

	[ObservableProperty]
	private string header;

	[ObservableProperty]
	private int index;

	[ObservableProperty]
	private IRelayCommand<object> removeCommand;

	public CardViewModel(IRelayCommand<object> removeCommand, string header = "", int index = 0) {
		RemoveCommand = removeCommand;
		Header = header;
		Index = index;
	}

}
