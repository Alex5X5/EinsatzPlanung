namespace Einsatzplanung.GUI.ViewModels.Edit;

using System;
using System.Collections.Generic;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Avalonia.Media;

using Einsatzplanung.GUI.Interfaces;

public partial class SpecializationCardViewModel : ViewModelBase, IRemovableCardViewModel {

	[ObservableProperty]
	private string header;

	public IReadOnlyList<string> TopicNames { get; }

	public IRelayCommand<object> RemoveCommand {
		init; get;
	}

	public SpecializationCardViewModel(string header, IReadOnlyList<string> topicNames, IRelayCommand<object> removeCommand) {
		Header = header;
		TopicNames = topicNames;
		RemoveCommand = removeCommand;
	}
}