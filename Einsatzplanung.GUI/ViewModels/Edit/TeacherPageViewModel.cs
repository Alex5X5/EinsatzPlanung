namespace Einsatzplanung.GUI.ViewModels.Edit;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

using Einsatzplanung.Excel.Interfaces;
using Einsatzplanung.GUI.Interfaces;
using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models;
using Einsatzplanung.Types.Models.Configuration;

public partial class TeacherPageViewModel : ViewModelBase, IEditViewChild {

	private IConfigExportService<TeacherConfig> exportService;
	private IConfigService<TeacherConfig> configService;

	[ObservableProperty]
	private ObservableCollection<TeacherCardViewModel> cards;

	[ObservableProperty]
	private int activeCardIndex = -1;

	public TeacherPageViewModel(IConfigService<TeacherConfig> configService, IConfigExportService<TeacherConfig> exportService) {
		this.exportService = exportService;
		this.configService = configService;
		Cards = [];
	}

	public void AddCard() {
		Cards.Add(new TeacherCardViewModel(new TeacherConfig(){Name = "Neuer Ausbilder", Abbreviation="Au", Specializations=[], WeeklyHours=40}, Cards.Count));
	}

	public async Task ExportCardsAsync(string path) {
		await SetEntitiesFromStateAsync();
		await exportService.ExportAsync(path, configService.GetData());
	}

	protected override async Task OnLoadAsync() {
		List<TeacherCardViewModel> cards = await Task.Run(
			() => {
				var teachers = configService.GetData();
				int i = 0;
				return teachers
					.Select(t => new TeacherCardViewModel(t, i++))
					.ToList();
			});
		Cards = new(cards);
	}

	protected override async Task OnUnloadAsync() {
		await SetEntitiesFromStateAsync();
	}

	public async Task SetEntitiesFromStateAsync() {
		await Task.Run(() => {
			List<TeacherConfig> teachers = Cards.Select(card => new TeacherConfig {
				Name = card.Header,
				Abbreviation = card.Abbreviation,
				WeeklyHours = card.WeeklyHours,
				Specializations = card.Topics.Select(topic => new Topic(topic.Header)).ToList()
			}).ToList();

			configService.SetData(teachers);
		});
	}
}