namespace Einsatzplanung.GUI.ViewModels.Edit;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Einsatzplanung.Excel.Interfaces;
using Einsatzplanung.GUI.Interfaces;
using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models.Configuration;

public partial class TeacherPageViewModel : ViewModelBase, IEditViewChild {

	private IConfigExportService<TeacherConfig> exportService;
	private IConfigService<TeacherConfig> configService;
	private IConfigService<TopicConfig> topicService;

	public ObservableCollection<TeacherCardViewModel> Cards { set; get; }

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
			Name = "",
			Abbreviation = "Au",
			Specializations = [],
			WeeklyHours = 40
		};
		Cards.Add(new TeacherCardViewModel(teacher, RemoveCardCommand, Cards.Count));
	}

	[RelayCommand]
	private void RemoveCard(object card) {
		if (card is TeacherCardViewModel teacherCard)
			Cards.Remove(teacherCard);
	}

	protected override async Task OnLoadAsync(CancellationToken token) {
		List<TeacherCardViewModel> cards = await Task.Run(
			async () => {
				var teachers = await configService.GetDataAsync(token);
				int i = 0;
				return teachers
					.Select(t => new TeacherCardViewModel(t, RemoveCardCommand, i++))
					.ToList();
			});
		Cards.Clear();
		foreach (var card in cards)
			Cards.Add(card);
	}

	protected override async Task OnUnloadAsync(CancellationToken token) {
		await SetEntitiesFromStateAsync();
	}

	public async Task ExportCardsAsync(string path, CancellationToken? token = null) {
		await SetEntitiesFromStateAsync(token);
		var data = await configService.GetDataAsync(token);
		await exportService.ExportAsync(path, data);
	}

	public async Task SetEntitiesFromStateAsync(CancellationToken? token = null) {
		await Task.Run(async () => {
			List<TeacherConfig> teachers = [];
			foreach (var teacherCard in Cards) {
				List<TopicConfig> specializations = [];
				foreach (var specCard in teacherCard.Specializations) {
					var option = await topicService.TryGetByNameAsync(specCard.Header);
					if(option.Success)
						specializations.Add(option.Value!);
				}
				var teacher = new TeacherConfig() { 
					Id = teacherCard.Config.Id,
					Abbreviation = teacherCard.Abbreviation,
					Name = teacherCard.Header,
					Specializations = specializations,
					WeeklyHours = teacherCard.Config.WeeklyHours,
				};
				teacher.Name = teacherCard.Header;
				teachers.Add(teacher);
			}
			await configService.SetDataAsync(teachers);
		});
	}
}