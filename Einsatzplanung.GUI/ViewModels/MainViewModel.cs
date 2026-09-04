namespace Einsatzplanung.GUI.ViewModels;

using Microsoft.Extensions.DependencyInjection;
using CommunityToolkit.Mvvm.ComponentModel;

using EinsatzPlanung.GUI;

public partial class MainViewModel : ViewModelBase {

	[ObservableProperty]
	private ViewModelBase currentPage;
	
	private void ChangePage<T>() where T : ViewModelBase {
		CurrentPage = App.Current.Services.GetRequiredService<T>();
	}

	public MainViewModel() : base() {
		CurrentPage = App.Current.Services.GetRequiredService<ImportViewModel>();
	}
        
}