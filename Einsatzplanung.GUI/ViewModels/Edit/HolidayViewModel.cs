namespace Einsatzplanung.GUI.ViewModels.Edit;

using System;
using System.Collections.ObjectModel;
using System.Linq;

using CommunityToolkit.Mvvm.Input;

using Einsatzplanung.Excel.Interfaces;
using Einsatzplanung.Excel.Models;
using Einsatzplanung.GUI.Interfaces;
using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models;

public partial class HolidayViewModel : ViewModelBase, IEditViewChild {

	private IExcelExportService exportService;

	public string Header { get; }

	public ObservableCollection<HolidayCardViewModel> Cards { get; }

	public HolidayViewModel(IEntityService<Holiday> configService, IExcelExportService exportService) {
		this.exportService = exportService;
		var holidays = configService.GetEntities();
		Cards = new(holidays.Select(t => new HolidayCardViewModel($"{t.From:dd.MM.yyyy} - {t.To:dd.MM.yyyy}")));
	}
	
	public void AddCard() {
		Cards.Add(new HolidayCardViewModel($"{DateTime.Now.Date:dd.MM.yyyy} - {DateTime.Now.Date:dd.MM.yyyy}"));
	}

	public void ExportCards(string path) {
		Table table = new();

		table.AddRow(["Ausbildungsgruppe", "Klasse", "Schulwoche", "Thema", "Themenwochen", "Farbe"]);

		foreach (var card in Cards) {
			//table.AddRow([
			//	firstTopic ? card.Header : "",
			//	firstTopic ? card.Header : "",
			//	"",
			//	topic.Topic,
			//	topic.Count,
			//	"#FFFFFF"
			//]);
		}

		exportService.SaveTable(path, table);
	}
}