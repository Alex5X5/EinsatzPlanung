namespace Einsatzplanung.GUI.ViewModels;

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;

using Einsatzplanung.GUI.ViewModels.Edit;
using Microsoft.Extensions.DependencyInjection;

using System;

public partial class EditViewModel : ViewModelBase
{
	public ClassesPageViewModel ClassesPage { get; } = new();
	public TeacherPageViewModel TeacherPage { get; } = new();

	[ObservableProperty]
	private int selectedPageIndex;

	[RelayCommand]
	private void OnPrintCurrentPage()
	{
		Console.WriteLine(SelectedPageIndex == 0 ? "Schulklassen" : "Ausbilder");
		if (SelectedPageIndex == 0)
		{
			foreach (var card in ClassesPage.Cards)
			{
				Console.WriteLine(card.Header);
				foreach (var topic in card.Topics)
					Console.WriteLine($"- {topic.Topic}");
			}
		}
		else {
			foreach (var card in TeacherPage.Cards)
			{
				Console.WriteLine(card.Header);
				foreach (var topic in card.Topics)
					Console.WriteLine($"- {topic.Topic}");
			}
		}
		
	}

	[RelayCommand]
	private void OnGoToNextPage()
	{
		App.Current.Services.GetRequiredService<MainViewModel>().ChangePage<SaveViewModel>();
	}
}