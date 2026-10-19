namespace Einsatzplanung.GUI.ViewModels.Edit;

using System;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Avalonia.Media;

using Einsatzplanung.GUI.Interfaces;

public partial class SpecializationCardViewModel : ViewModelBase, IRemovableCardViewModel {

	[ObservableProperty]
	private string header;

	public IRelayCommand<object> RemoveCommand {
		init; get;
	}

	public SpecializationCardViewModel(string header, IRelayCommand<object> removeCommand) {
		Header = header;
		RemoveCommand = removeCommand;
	}
}