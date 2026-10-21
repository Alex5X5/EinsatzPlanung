namespace Einsatzplanung.GUI.ViewModels.Edit;

using System;
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
	private IConfigService<TopicConfig> topicService;

	[ObservableProperty]
	private ObservableCollection<TeacherCardViewModel> cards;

	[ObservableProperty]
	private int activeCardIndex = -1;

	public TeacherPageViewModel(IConfigService<TeacherConfig> configService, IConfigExportService<TeacherConfig> exportService, IConfigService<TopicConfig> topicService) {
		this.exportService = exportService;
		this.configService = configService;
		this.topicService = topicService;
		Cards = [];
	}

	public void AddCard() {
		var teacher = new TeacherConfig() {
			Id = Guid.NewGuid(), 
			Name = "Neuer Ausbilder",
			Abbreviation = "Au",
			Specializations = [],
			WeeklyHours = 40
		};
		Cards.Add(new TeacherCardViewModel(teacher, Cards.Count));
	}

	protected override async Task OnLoadAsync() {
		await configService.UpdateReferencesAsync();
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

	public async Task ExportCardsAsync(string path) {
		await SetEntitiesFromStateAsync();
		await exportService.ExportAsync(path, configService.GetData());
	}

	public async Task SetEntitiesFromStateAsync() {
		await Task.Run(() => {
			List<TeacherConfig> teachers = [];
			foreach (var teacherCard in Cards) {
				List<TopicConfig> specializations = [];
				foreach (var specCard in teacherCard.Topics) {
					specializations.Add(topicService.GetByName(specCard.Header));
				}
				var teacher = new TeacherConfig() { 
					Id = teacherCard.Config.Id,
					Abbreviation = teacherCard.Config.Abbreviation,
					Name = teacherCard.Header,
					Specializations = specializations,
					WeeklyHours = teacherCard.Config.WeeklyHours,
				};
				teacher.Name = teacherCard.Header;
				teachers.Add(teacher);
			}
			configService.SetData(teachers);
		});
	}
}