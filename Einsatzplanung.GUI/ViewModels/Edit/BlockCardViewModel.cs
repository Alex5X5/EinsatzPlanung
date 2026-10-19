namespace Einsatzplanung.GUI.ViewModels.Edit;

using System;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Avalonia.Media;

using Einsatzplanung.GUI.Interfaces;

public partial class BlockCardViewModel : ViewModelBase, IRemovableCardViewModel {

	[ObservableProperty]
	private string header;

	[ObservableProperty]
	private string count;

	public IRelayCommand<object> RemoveCommand {
		init; get;
	}

	public BlockCardViewModel(string header, int count, IRelayCommand<object> removeCommand) {
		Header = header;
		Count = Convert.ToString(count);
		RemoveCommand = removeCommand;
	}
}