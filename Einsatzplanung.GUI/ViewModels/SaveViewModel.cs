namespace Einsatzplanung.GUI.ViewModels;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Einsatzplanung.Excel.Interfaces;
using Einsatzplanung.Generation.Interfaces;
using Einsatzplanung.Types.Models.Generation;
using Einsatzplanung.Util.Services;

using Microsoft.Extensions.DependencyInjection;

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public partial class SaveViewModel : ViewModelBase {

	[ObservableProperty]
	private string selectedFolderPath = GetDefaultDownloadsFolder();

	[ObservableProperty]
	private string errorString = "";

	[RelayCommand]
	private async Task SavePath() {
		var window = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
		if (window is null) {
			Console.WriteLine("Kein Fenster gefunden");
			return;
		}

		var folders = await window.StorageProvider.OpenFolderPickerAsync(
			new FolderPickerOpenOptions {
				Title = "Ordner auswählen",
				AllowMultiple = false
			});

		var folder = folders.FirstOrDefault();
		SelectedFolderPath = folder?.Path.LocalPath ?? SelectedFolderPath;
		Console.WriteLine(SelectedFolderPath);
	}

	[RelayCommand]
	private async Task Export() {
		try {
			await Task.Run(
				() => {
					Plan plan = App.Current.Services.GetRequiredService<IGeneratorService>().GeneratePlan();
					App.Current.Services.GetRequiredService<IPlanExportService>().ExportPlan(GetFileName(), plan);
				});
		} catch(InvalidOperationException e) {
			ErrorString = e.Message;
		}
	}

	private static string GetDefaultDownloadsFolder() {
		var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
		return Path.Combine(userProfile, "Downloads");
	}

	private string GetFileName() {
		int year = App.Current.Services.GetRequiredService<GeneralConfigService>().YearStartDate.Year;
		return Path.Join(SelectedFolderPath, $"Einsatzplan_{year}_{year+1}.xlsx");
	}
}