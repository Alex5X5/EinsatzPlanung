namespace Einsatzplanung.GUI.ViewModels;

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;

using Einsatzplanung.GUI.ViewModels.Edit;
using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Einsatzplanung.Excel.Models;
using Einsatzplanung.Excel.Services;

using Avalonia.Platform.Storage;
using Avalonia.Controls;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

public partial class EditViewModel : ViewModelBase
{
	public ClassesPageViewModel ClassesPage { get; }
	public TeacherPageViewModel TeacherPage { get; }
	public HolidayViewModel HolidayPage { get; }

	private readonly ExcelExportService excelExportService;

	public EditViewModel(ExcelExportService excelExportService)
	{
		ClassesPage = App.Current.Services.GetRequiredService<ClassesPageViewModel>();
		TeacherPage = App.Current.Services.GetRequiredService<TeacherPageViewModel>();
		HolidayPage = App.Current.Services.GetRequiredService<HolidayViewModel>();
		this.excelExportService = excelExportService;
	}

	[ObservableProperty]
	private int selectedPageIndex;

	[RelayCommand]
	private async Task OnPrintCurrentPageAsync(Window? window)
	{
		if (window is null)
			return;

		var headers = SelectedPageIndex == 0
			? ClassesPage.Cards.Select(card => card.Header)
			: TeacherPage.Cards.Select(card => card.Header);
		
		var table = new Table { Index = 1 };

		if (SelectedPageIndex == 0)
		{
			table.Cells.Add(CreateRow("Ausbildungsgruppe", "Klasse", "Schulwoche", "Thema", "Themenwochen", "Farbe"));
			foreach (var card in ClassesPage.Cards)
			{
				var firstTopic = true;
				foreach (var topic in card.Topics)
				{
					table.Cells.Add(CreateRow(
						firstTopic ? card.Header : "",
						firstTopic ? card.Header : "",
						"",
						topic.Topic,
						"1",
						"#FFFFFF"));
					firstTopic = false;
				}
			}
		}
		else
		{
			table.Cells.Add(CreateRow("Name", "Kürzel", "Spezialisierung", "Wochenstunden"));
			foreach (var card in TeacherPage.Cards)
			{
				var firstTopic = true;
				foreach (var topic in card.Topics)
				{
					table.Cells.Add(CreateRow(
						firstTopic ? card.Header : "",
						firstTopic ? card.Header : "",
						topic.Topic,
						"40"));
					firstTopic = false;
				}
			}
		}
		

        var file = await window.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Export als Excel speichern",
            SuggestedFileName = SelectedPageIndex == 0 ? "Schulklassen.xlsx" : "Ausbilder.xlsx",
            FileTypeChoices = new[]
            {
                new FilePickerFileType("Excel-Datei")
                {
                    Patterns = new[] { "*.xlsx" }
                }
            }
        });

        if (file is null)
            return;

		excelExportService.SaveTable(file.Path.LocalPath, table, 1, "Export");
		
	}

	private static List<TableCell> CreateRow(params string[] values)
	{
		return values.Select(value => new TableCell { Value = value }).ToList();
	}

	[RelayCommand]
	private void OnGoToNextPage()
	{
		App.Current.Services.GetRequiredService<MainViewModel>().ChangePage<SaveViewModel>();
	}
}