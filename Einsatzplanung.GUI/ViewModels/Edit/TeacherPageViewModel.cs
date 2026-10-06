namespace Einsatzplanung.GUI.ViewModels.Edit;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Einsatzplanung.Excel.Interfaces;
using Einsatzplanung.Excel.Models;
using Einsatzplanung.GUI.Interfaces;
using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models;
using Einsatzplanung.Types.Models.Configuration;

public partial class TeacherPageViewModel : ViewModelBase, IEditViewChild {

	private IExcelExportService exportService;
	private IConfigService<TeacherConfig> configService;

	public ObservableCollection<TeacherCardViewModel> Cards { get; }

	public TeacherPageViewModel(IConfigService<TeacherConfig> configService, IExcelExportService exportService) {
		this.exportService = exportService;
		this.configService = configService;
		var teachers = configService.ParseSource();
		Cards = new(teachers.Select(t => new TeacherCardViewModel(t.Name, t.Specializations)));
	}

	public void AddCard() {
		Cards.Add(new TeacherCardViewModel("Neuer Ausbilder", []));
	}

	public void ExportCards(string path) {
		Table table = new();

		table.AddRow(["Name", "Kürzel", "Themen", "Wochenstunden"]);

		foreach (var card in Cards) {
			var firstTopic = true;
			foreach (var topic in card.Topics) {
				table.AddRow([
					firstTopic ? card.Header : "",
					firstTopic ? card.Header : "",
					topic.Header,
					"40"
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
		List<TeacherConfig> configs = [];
		configService.SetData(configs);
	}
}