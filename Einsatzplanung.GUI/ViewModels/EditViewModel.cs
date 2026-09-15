using CommunityToolkit.Mvvm.Input;

using EinsatzPlanung.GUI;

using Microsoft.Extensions.DependencyInjection;

namespace Einsatzplanung.GUI.ViewModels;

public partial class EditViewModel : ViewModelBase
{

	[RelayCommand]
	private void OnGoToNextPage()
	{
		App.Current.Services.GetRequiredService<MainViewModel>().ChangePage<SaveViewModel>();
	}
}