namespace Einsatzplanung.GUI.ViewModels.Edit;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Einsatzplanung.GUI.Interfaces;
using Einsatzplanung.Types.Models.Configuration;

using System;

public partial class BlockCardViewModel : ViewModelBase, IRemovableCardViewModel {

	[ObservableProperty]
	private string header;

	[ObservableProperty]
	private string count;

	public IRelayCommand<object> RemoveCommand { init; get; }
	
	public BlockConfig Config { init; get; }

	public BlockCardViewModel(BlockConfig block, IRelayCommand<object> removeCommand) {
		Config = block;
		Header = block.Topic.Name;
		Count = Convert.ToString(block.Count);
		RemoveCommand = removeCommand;
	}
}