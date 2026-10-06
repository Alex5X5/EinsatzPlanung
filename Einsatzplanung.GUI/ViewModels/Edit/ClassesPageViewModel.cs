namespace Einsatzplanung.GUI.ViewModels.Edit;

using Einsatzplanung.Excel.Interfaces;
using Einsatzplanung.Excel.Models;
using Einsatzplanung.GUI.Interfaces;
using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models.Configuration;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

public partial class ClassesPageViewModel : ViewModelBase, IEditViewChild {

	private IExcelExportService exportService;
	private IConfigService<GroupConfig> configService;

	public ObservableCollection<ClassCardViewModel> Cards { get; }

	public ClassesPageViewModel(IConfigService<GroupConfig> configService, IExcelExportService exportService) {
		this.exportService = exportService;
		this.configService = configService;
		var groups = configService.ParseSource();
		Cards = new(groups.Select(g => new ClassCardViewModel(g.Name, g.Blocks)));
	}

	public void AddCard() {
		Cards.Add(new ClassCardViewModel("Neue Klasse", []));
	}

	public void ExportCards(string path) {
		Table table = new();

		table.AddRow(["Lehrjahr", "Klasse", "Ausbilder", "Schulwoche", "Thema", "Wochen", "Farbe"]);
		
		foreach (var card in Cards) {
			var firstTopic = true;
			foreach (var topic in card.Topics) {
				table.AddRow([
					firstTopic ? card.Header : "",
					firstTopic ? card.Header : "",
					"",
					topic.Header,
					topic.Count,
					"#FFFFFF"
				]);
				firstTopic = false;
			}
		}

		exportService.SaveTable(path, table);
	}

	public override void OnUnload() {
		SetEntitiesFromState();
	}


	public void SetEntitiesFromState() {
		List<GroupConfig> groups = new();

		foreach (var card in Cards) {

			var group = new GroupConfig() {
				Name = "",
				TeacherAbbreviation = "",
				SchoolWeeks = [],
				Blocks = card.Topics.Select(
					t => new BlockConfig() {
						Name = t.Header,
						Count = int.Parse(t.Count),
						Color = "#FF0000"
					}).ToList()
			};

			groups.Add(group);
		}

		configService.SetData(groups);
	}
}