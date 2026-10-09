namespace Einsatzplanung.GUI.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using System.ComponentModel;
using System.Threading.Tasks;

public partial class ViewModelBase : ObservableObject, INotifyPropertyChanged {
	
	[RelayCommand]
	protected virtual Task OnLoadAsync() {
		return Task.CompletedTask;
	}

	[RelayCommand]
	protected virtual Task OnUnloadAsync() {
		return Task.CompletedTask;
	}
}
