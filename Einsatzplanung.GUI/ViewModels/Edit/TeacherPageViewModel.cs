namespace Einsatzplanung.GUI.ViewModels.Edit;

using System.Collections.ObjectModel;
using System.Linq;

using CommunityToolkit.Mvvm.Input;

using Einsatzplanung.Excel.Interfaces;
using Einsatzplanung.Excel.Models;
using Einsatzplanung.GUI.Interfaces;
using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models;

public partial class TeacherPageViewModel : ViewModelBase, IEditViewChild {

	private IExcelExportService exportService;

	public ObservableCollection<TeacherCardViewModel> Cards { get; }

	public TeacherPageViewModel(IEntityService<Teacher> configService, IExcelExportService exportService) {
		this.exportService = exportService;
		var teachers = configService.GetEntities();
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
					topic.Topic,
					"40"
				]);
				firstTopic = false;
			}
		}

		exportService.SaveTable(path, table);
	}
}