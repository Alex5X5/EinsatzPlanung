using Avalonia.Media;

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
	    
	[ObservableProperty]
	private Color selectedColor = Colors.DarkRed;

	public IRelayCommand<object> RemoveCommand { init; get; }

    public TopicCardViewModel(string header, int count, IRelayCommand<object> removeCommand)
		: this (header, Colors.White, count, removeCommand) {  }

	public TopicCardViewModel(string header, Color selectedColor, int count, IRelayCommand<object> removeCommand) {
		Header = header;
		SelectedColor = selectedColor;
		Count = Convert.ToString(count);
		RemoveCommand = removeCommand;
	}
}
