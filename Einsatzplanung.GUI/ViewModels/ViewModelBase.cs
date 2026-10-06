namespace Einsatzplanung.GUI.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using System.ComponentModel;

public partial class ViewModelBase : ObservableObject, INotifyPropertyChanged {
	
	[RelayCommand]
	public virtual void OnLoad() { }

	[RelayCommand]
	public virtual void OnUnload() { }
}
