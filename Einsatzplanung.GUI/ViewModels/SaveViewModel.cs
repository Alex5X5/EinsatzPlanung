namespace Einsatzplanung.GUI.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Einsatzplanung.Excel.Interfaces;
using Einsatzplanung.Generation.Interfaces;
using Einsatzplanung.GUI.Services;
using Einsatzplanung.Types.Models.Generation;
using Einsatzplanung.Util.Services;

using Microsoft.Extensions.DependencyInjection;

using System;
using System.IO;
using System.Threading.Tasks;

public partial class SaveViewModel : ViewModelBase {

	[ObservableProperty]
	private string selectedFolderPath = GetDefaultDownloadsFolder();

	[ObservableProperty]
	private string errorString = "";

	[RelayCommand]
	private async Task SavePath() {
		SelectedFolderPath = (await FilePickerService.PickFolder("Ordner auswählen")) ?? SelectedFolderPath;
		Console.WriteLine(SelectedFolderPath);
	}

	[RelayCommand]
	private async Task Export() {
		try {
			await Task.Run(
				() => {
					var generator = App.Current.Services.GetRequiredService<IGeneratorService>();
					var export = App.Current.Services.GetRequiredService<IPlanExportService>();
					var plan = generator.GeneratePlan();
					export.ExportPlan(GetFileName(), plan);
				});
		} catch(Exception e) {
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