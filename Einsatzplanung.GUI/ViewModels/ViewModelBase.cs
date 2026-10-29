namespace Einsatzplanung.GUI.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

public partial class ViewModelBase : ObservableObject, INotifyPropertyChanged {
	
	[RelayCommand]
	protected virtual Task OnLoadAsync(CancellationToken token) {
		return Task.CompletedTask;
	}

	[RelayCommand]
	protected virtual Task OnUnloadAsync(CancellationToken token) {
		return Task.CompletedTask;
	}
}
