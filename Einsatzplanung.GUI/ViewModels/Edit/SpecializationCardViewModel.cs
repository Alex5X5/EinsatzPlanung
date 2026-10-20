namespace Einsatzplanung.GUI.ViewModels.Edit;

using Avalonia.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Einsatzplanung.GUI.Interfaces;
using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models.Configuration;

using System;
using System.Collections.Generic;

public partial class SpecializationCardViewModel : ViewModelBase, IRemovableCardViewModel {

	[ObservableProperty]
	private string header;

	public IReadOnlyList<string> TopicNames { get; }

	public IRelayCommand<object> RemoveCommand { init; get; }

	public SpecializationCardViewModel(TopicConfig config, List<TopicConfig> topics,IRelayCommand<object> removeCommand) {
		Header = config.Name;
		RemoveCommand = removeCommand;
	}
}