namespace Einsatzplanung.GUI.ViewModels;

using Microsoft.Extensions.DependencyInjection;
using CommunityToolkit.Mvvm.ComponentModel;

using Einsatzplanung.GUI;

public partial class MainViewModel : ViewModelBase {

	[ObservableProperty]
	private ViewModelBase currentPage;
	
	// Check if the current page is the import/edit/save page
	public bool IsImportPageActive => CurrentPage is ImportViewModel;
	public bool IsEditPageActive => CurrentPage is EditViewModel;
	public bool IsSavePageActive => CurrentPage is SaveViewModel;

	// Check again when the current page changes
	partial void OnCurrentPageChanged(ViewModelBase value) {
		OnPropertyChanged(nameof(IsImportPageActive));
		OnPropertyChanged(nameof(IsEditPageActive));
		OnPropertyChanged(nameof(IsSavePageActive));
	}

	public void ChangePage<T>() where T : ViewModelBase {
		CurrentPage = App.Current.Services.GetRequiredService<T>();
	}

	public MainViewModel() : base() {
		CurrentPage = App.Current.Services.GetRequiredService<ImportViewModel>();
	}
        
}