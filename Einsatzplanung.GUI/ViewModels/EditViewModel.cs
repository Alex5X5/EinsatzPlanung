namespace Einsatzplanung.GUI.ViewModels;

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;

using Einsatzplanung.GUI.ViewModels.Edit;
using Microsoft.Extensions.DependencyInjection;

using Avalonia.Platform.Storage;
using Avalonia.Controls;
using System.Threading.Tasks;
using Einsatzplanung.GUI.Interfaces;
using System;
using Einsatzplanung.GUI.Services;

public partial class EditViewModel : ViewModelBase {

	public ClassesPageViewModel ClassesPage { get; }
	public TeacherPageViewModel TeacherPage { get; }
	public HolidayViewModel HolidayPage { get; }

	[ObservableProperty]
	private IEditViewChild currentPage;

	public int SelectedPageIndex {
		set {
			switch(value) {
				case 0: {
					ChangePage<ClassesPageViewModel>();
					break;
				}
				case 1: {
					ChangePage<TeacherPageViewModel>();
					break;
				}
				case 2: {
					ChangePage<HolidayViewModel>();
					break;
				}
				default: {
					throw new InvalidOperationException($"Can not change to index {value}");
				}
			}
			OnPropertyChanged(nameof(SelectedPageIndex));
		}
		get {
			if(CurrentPage.GetType() == typeof(ClassesPageViewModel))
				return 0;
			if (CurrentPage.GetType() == typeof(TeacherPageViewModel))
				return 1;
			if (CurrentPage.GetType() == typeof(HolidayViewModel))
				return 2;
			return 0;
		}	
	}
	
	public EditViewModel() {
		SelectedPageIndex = 0;
	}

	private void ChangePage<T>() where T : IEditViewChild {
		CurrentPage = App.Current.Services.GetRequiredService<T>();
	}

	[RelayCommand]
	private async Task OnPrintCurrentPageAsync(Window? window) {
		if (window is null)
			return;

		var file = await FilePickerService.PickSaveLocation(
			new FilePickerSaveOptions {
				Title = "Export als Excel speichern",
				SuggestedFileName = SelectedPageIndex == 0 ? "Schulklassen.xlsx" : "Ausbilder.xlsx",
				FileTypeChoices = [
					new FilePickerFileType("Excel-Datei") {
						Patterns = [ "*.xlsx" ]
					}
				]
			});

		await Task.Run(
			()=>{
				if (file is not null)
					CurrentPage.ExportCards(file);
			});
	}

	[RelayCommand]
	private void OnAdd() {
		CurrentPage.AddCard();
	}

	[RelayCommand]
	private void OnGoToNextPage() {
		App.Current.Services.GetRequiredService<MainViewModel>().ChangePage<SaveViewModel>();
	}
}