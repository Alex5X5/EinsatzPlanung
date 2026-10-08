namespace Einsatzplanung.GUI.ViewModels.Edit;

using CommunityToolkit.Mvvm.ComponentModel;

using Einsatzplanung.Excel.Interfaces;
using Einsatzplanung.GUI.Interfaces;
using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models.Configuration;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

public partial class ClassesPageViewModel : ViewModelBase, IEditViewChild {

	private IConfigExportService<GroupConfig> exportService;
	private IConfigService<GroupConfig> configService;

	public ObservableCollection<ClassCardViewModel> Cards { get; }

	[ObservableProperty]
	private int activeCardIndex = -1;

	public ClassesPageViewModel(IConfigService<GroupConfig> configService, IConfigExportService<GroupConfig> exportService) {
		this.exportService = exportService;
		this.configService = configService;
		var groups = configService.GetData();
		int i = 0;
		Cards = new(groups.Select(g => new ClassCardViewModel(g, i++)));
	}

	public void AddCard() {
		var group = new GroupConfig() {
			Name = "Neue Klasse",
			AgeGroupName = "",
			TeacherAbbreviation = "",
			SchoolWeeks = [],
			Blocks = []
		};

		Cards.Add(new ClassCardViewModel(group, Cards.Count));
	}

	public void ExportCards(string path) {
		SetEntitiesFromState();
		exportService.Export(path, configService.GetData());
	}

	public override void OnUnload() {
		SetEntitiesFromState();
	}

	public void SetEntitiesFromState() {
		List<GroupConfig> groups = [];

		foreach (var card in Cards) {
			var blocks = card.Topics.Select(
				t => new BlockConfig() {
					Name = t.Header,
					Count = int.Parse(t.Count),
					Color = "#FF0000"
				}).ToList();
			var group = new GroupConfig() {
				Name = card.Header,
				AgeGroupName = card.Config.AgeGroupName,
				TeacherAbbreviation = card.Config.TeacherAbbreviation,
				SchoolWeeks = card.Config.SchoolWeeks,
				Blocks = blocks
			};
			groups.Add(group);
		}

		configService.SetData(groups);
	}
}