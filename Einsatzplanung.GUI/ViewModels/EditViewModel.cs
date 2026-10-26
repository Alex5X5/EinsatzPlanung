namespace Einsatzplanung.GUI.ViewModels;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Avalonia.Controls;
using Avalonia.Platform.Storage;

using Einsatzplanung.GUI.Interfaces;
using Einsatzplanung.GUI.Services;
using Einsatzplanung.GUI.ViewModels.Edit;
using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models.Configuration;

using Microsoft.Extensions.DependencyInjection;

public partial class EditViewModel : ViewModelBase {

	[ObservableProperty]
	private IEditViewChild currentPage;

	[ObservableProperty]
	private bool hasLoaded = false;

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
			if (CurrentPage?.GetType() == typeof(TopicViewModel))
				return 0;
			if (CurrentPage?.GetType() == typeof(AgeGroupPageViewModel))
				return 1;
			if (CurrentPage?.GetType() == typeof(TeacherPageViewModel))
				return 2;
			if (CurrentPage?.GetType() == typeof(HolidayViewModel))
				return 3;
			return 0;
		}	
	}
	
	public EditViewModel() {

		_ = Task.Run(
			() => {
				List<TopicConfig> topics = App.GetRequiredService<IConfigService<TopicConfig>>().GetData();
				Console.WriteLine($"loaded topics:[\n   {string.Join(",\n   ", topics.Select(x => x.ToString()))}\n]");
				var teachers = App.GetRequiredService<IConfigService<TeacherConfig>>().GetData();
				Console.WriteLine($"loaded teachers:[\n   {string.Join(",\n   ", teachers.Select(x => x.ToString()))}\n]");
				var groups = App.GetRequiredService<IConfigService<AgeGroupConfig>>().GetData();
				Console.WriteLine($"loaded agegroups:[\n   {string.Join(",\n   ", groups.Select(x => x.ToString()))}\n]");
				HasLoaded = true;
				SelectedPageIndex = 0;
			}
		);
	}

	private void ChangePage<T>() where T : IEditViewChild {
		CurrentPage = App.GetRequiredService<T>();
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