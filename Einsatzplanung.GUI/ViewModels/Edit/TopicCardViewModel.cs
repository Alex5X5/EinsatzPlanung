namespace Einsatzplanung.GUI.ViewModels.Edit;

using System;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Einsatzplanung.GUI.Interfaces;

public partial class TopicCardViewModel : ViewModelBase, IRemovableCardViewModel {
	
	[ObservableProperty]
	private string header;

	[ObservableProperty]
	private string count;

	public IRelayCommand<object> RemoveCommand { init; get; }

    public TopicCardViewModel(string header, int count, IRelayCommand<object> removeCommand) {
        Header = header;
		Count = Convert.ToString(count);
		RemoveCommand = removeCommand;
    }
}
