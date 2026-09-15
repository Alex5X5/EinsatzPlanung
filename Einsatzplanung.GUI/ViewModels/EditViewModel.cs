namespace Einsatzplanung.GUI.ViewModels;

using CommunityToolkit.Mvvm.Input;

using Microsoft.Extensions.DependencyInjection;


public partial class EditViewModel : ViewModelBase
{

	[RelayCommand]
	private void OnGoToNextPage()
	{
		App.Current.Services.GetRequiredService<MainViewModel>().ChangePage<SaveViewModel>();
	}
}