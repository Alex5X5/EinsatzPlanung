namespace Einsatzplanung.GUI.Interfaces; 

using CommunityToolkit.Mvvm.Input;

public interface IRemovableCardViewModel {

	public IRelayCommand<object> RemoveCommand { get; }

	public string Header { get; }

}
