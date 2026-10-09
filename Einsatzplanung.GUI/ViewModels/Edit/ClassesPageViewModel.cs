namespace Einsatzplanung.GUI.ViewModels.Edit;

using CommunityToolkit.Mvvm.ComponentModel;

using Einsatzplanung.Excel.Interfaces;
using Einsatzplanung.GUI.Interfaces;
using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models.Configuration;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

public partial class ClassesPageViewModel : ViewModelBase, IEditViewChild {

	private IConfigExportService<GroupConfig> exportService;
	private IConfigService<GroupConfig> configService;

	[ObservableProperty]
	private ObservableCollection<ClassCardViewModel> cards;

	[ObservableProperty]
	private int activeCardIndex = -1;

	public ClassesPageViewModel(IConfigService<GroupConfig> configService, IConfigExportService<GroupConfig> exportService) {
		this.exportService = exportService;
		this.configService = configService;
		Cards = [];
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

	public async Task ExportCardsAsync(string path) {
		await SetEntitiesFromStateAsync();
		await exportService.ExportAsync(path, configService.GetData());
	}

	protected override async Task OnLoadAsync() {
		List<ClassCardViewModel> cards = await Task.Run(() => {
			var groups = configService.GetData();
			int i = 0;
			return groups
				.Select(g => new ClassCardViewModel(g, i++))
				.ToList();
		});
		Cards = new ObservableCollection<ClassCardViewModel>(cards);
	}

	protected override async Task OnUnloadAsync() {
		await SetEntitiesFromStateAsync();
	}

	public async Task SetEntitiesFromStateAsync() {
		await Task.Run(() => {
			List<AgeGroupConfig> ageGroups = [];
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
		});
	}
}