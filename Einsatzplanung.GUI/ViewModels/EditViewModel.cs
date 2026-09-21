namespace Einsatzplanung.GUI.ViewModels;

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;

using Einsatzplanung.GUI.ViewModels.Edit;
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
	public ClassesPageViewModel ClassesPage { get; } = new();
	public TeacherPageViewModel TeacherPage { get; } = new();

	private readonly ExcelExportService excelExportService = new();

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

		foreach (var header in headers) {
			table.Cells.Add(new List<TableCell>
			{
				new() { Value = header }
			});
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

		excelExportService.SaveTableToFile(file.Path.LocalPath, table);
		
	}

	[RelayCommand]
	private void OnGoToNextPage()
	{
		App.Current.Services.GetRequiredService<MainViewModel>().ChangePage<SaveViewModel>();
	}
}