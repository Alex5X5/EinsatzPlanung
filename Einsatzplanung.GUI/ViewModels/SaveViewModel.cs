namespace Einsatzplanung.GUI.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Einsatzplanung.Excel.Interfaces;
using Einsatzplanung.Generation.Interfaces;
using Einsatzplanung.GUI.Services;
using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models;
using Einsatzplanung.Types.Models.Generation;
using Einsatzplanung.Util.Services;

using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public partial class SaveViewModel : ViewModelBase {

	public System.Collections.ObjectModel.ObservableCollection<ClassSelectionItem> ClassesNamesForSave { get; } = new();

	[ObservableProperty]
	private string selectedFolderPath = PathService.DOWNLOADS_DIRECTORY;

	[ObservableProperty]
	private string errorString = "";

	public SaveViewModel() {
	}

	protected override async Task OnLoadAsync(CancellationToken token) {
		var items = await Task.Run(
			() => {
				var groupService = App.GetRequiredService<IEntityService<Group>>();
				var groups = groupService.GetEntities();
				return groups
					.Select(
						x =>
							new ClassSelectionItem {
								Name = x.Name,
								IsSelected = true,
								Config = x
							})
					.ToList();
			});

		ClassesNamesForSave.Clear();
		foreach (var item in items) {
			ClassesNamesForSave.Add(item);
		}
	}


	[RelayCommand]
	private async Task SavePath() {
		string? path = await FilePickerService.PickFolder("Ordner auswählen");
		if(!string.IsNullOrEmpty(path))
			SelectedFolderPath = path!;
	}

	[RelayCommand]
	private async Task Export() {
		try {
			await Task.Run(
				() => {
					var generator = App.Current.Services.GetRequiredService<IGeneratorService>();
					var export = App.Current.Services.GetRequiredService<IPlanExportService>();
					var config = new GeneratorConfig() {
						GroupsToExport = ClassesNamesForSave.Select(card => new KeyValuePair<Guid, bool>(card.Config.Id, card.IsSelected)).ToDictionary()
					};
					var plan = generator.GeneratePlan(config);
					export.ExportPlan(GetFileName(), plan);
				});
		} catch(Exception e) {
			ErrorString = e.Message;
		}
	}

	private string GetFileName() {
		int year = App.Current.Services.GetRequiredService<GeneralConfigService>().YearStartDate.Year;
		return Path.Join(SelectedFolderPath, $"Einsatzplan_{year}_{year+1}.xlsx");
	}
}