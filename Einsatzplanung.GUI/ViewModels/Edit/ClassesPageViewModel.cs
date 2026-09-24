namespace Einsatzplanung.GUI.ViewModels.Edit;

using System.Collections.ObjectModel;
using System.Linq;

using CommunityToolkit.Mvvm.Input;

using Einsatzplanung.Excel.Interfaces;
using Einsatzplanung.Excel.Models;
using Einsatzplanung.GUI.Interfaces;
using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models;

public partial class ClassesPageViewModel : ViewModelBase, IEditViewChild {

	private IExcelExportService exportService;

	public ObservableCollection<ClassCardViewModel> Cards { get; }

	public ClassesPageViewModel(IEntityService<Group> configService, IExcelExportService exportService) {
		this.exportService = exportService;
		var groups = configService.GetEntities();
		Cards = new(groups.Select(g => new ClassCardViewModel(g.Name, g.Blocks)));
	}

	public void AddCard() {
		Cards.Add(new ClassCardViewModel("Neue Klasse", []));
	}

	public void ExportCards(string path) {
		Table table = new();

		table.AddRow(["Ausbildungsgruppe", "Klasse", "Schulwoche", "Thema", "Themenwochen", "Farbe"]);
		
		foreach (var card in Cards) {
			var firstTopic = true;
			foreach (var topic in card.Topics) {
				table.AddRow([
					firstTopic ? card.Header : "",
					firstTopic ? card.Header : "",
					"",
					topic.Topic,
					topic.Count,
					"#FFFFFF"
				]);
				firstTopic = false;
			}
		}

		exportService.SaveTable(path, table);
	}
}