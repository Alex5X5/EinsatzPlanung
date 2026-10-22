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

	[ObservableProperty]
	private IEditViewChild currentPage;

	public int SelectedPageIndex {
		set {
			switch(value) {
				case 0: {
					ChangePage<TopicViewModel>();
					break;
				}
				case 1: {
					ChangePage<AgeGroupPageViewModel>();
					break;
				}
				case 2: {
					ChangePage<TeacherPageViewModel>();
					break;
				}
				case 3: {
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
			if (CurrentPage.GetType() == typeof(TopicViewModel))
				return 0;
			if (CurrentPage.GetType() == typeof(AgeGroupPageViewModel))
				return 1;
			if (CurrentPage.GetType() == typeof(TeacherPageViewModel))
				return 2;
			if (CurrentPage.GetType() == typeof(HolidayViewModel))
				return 3;
			return 0;
		}	
	}
	
	public EditViewModel() {
		SelectedPageIndex = 0;
	}

	private void ChangePage<T>() where T : IEditViewChild {
		try {
			CurrentPage = App.Current.Services.GetService<T>();
		} catch(Exception e){
			Console.WriteLine("");
		}
	}

	[RelayCommand]
	private async Task OnExport() {
		var file = await FilePickerService.PickSaveLocation(
			new FilePickerSaveOptions {
				Title = "Export als Excel speichern",
				SuggestedFileName = SelectedPageIndex switch {
					0 => "Themen.xlsx",
					1 => "Lehrjahre.xlsx",
					2 => "Ausbilder.xlsx",
					3 => "Urlaub.xlsx",
					_ => "Export.xlsx"
				},
				FileTypeChoices = [
					new FilePickerFileType("Excel-Datei") {
						Patterns = [ "*.xlsx" ]
					}
				]
			});

		if (file is not null)
			await CurrentPage.ExportCardsAsync(file);
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