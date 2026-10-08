namespace Einsatzplanung.GUI.ViewModels.Edit;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using CommunityToolkit.Mvvm.ComponentModel;

using Einsatzplanung.Excel.Interfaces;
using Einsatzplanung.GUI.Interfaces;
using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models;
using Einsatzplanung.Types.Models.Configuration;

public partial class TeacherPageViewModel : ViewModelBase, IEditViewChild {

	private IConfigExportService<TeacherConfig> exportService;
	private IConfigService<TeacherConfig> configService;

	public ObservableCollection<TeacherCardViewModel> Cards { get; }

	[ObservableProperty]
	private int activeCardIndex = -1;

	public TeacherPageViewModel(IConfigService<TeacherConfig> configService, IConfigExportService<TeacherConfig> exportService) {
		this.exportService = exportService;
		this.configService = configService;
		var teachers = configService.GetData();
		int i = 0;
		Cards = new(teachers.Select(t => new TeacherCardViewModel(t, i++)));
	}

	public void AddCard() {
		Cards.Add(new TeacherCardViewModel(new TeacherConfig(){Name = "Neuer Ausbilder", Abbreviation="Au", Specializations=[], WeeklyHours=40}, Cards.Count));
	}

	public void ExportCards(string path) {
		SetEntitiesFromState();
		exportService.Export(path, configService.GetData());
	}

	public override void OnUnload() {
		SetEntitiesFromState();
	}

	public void SetEntitiesFromState() {
		List<TeacherConfig> teachers = Cards.Select(card => new TeacherConfig {
			Name = card.Header,
			Abbreviation = card.Abbreviation,
			WeeklyHours = card.WeeklyHours,
			Specializations = card.Topics.Select(topic => new Topic(topic.Header)).ToList()
		}).ToList();

		configService.SetData(teachers);
	}
}