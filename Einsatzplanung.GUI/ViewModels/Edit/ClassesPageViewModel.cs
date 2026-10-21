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
using Einsatzplanung.Types.Models.Configuration;

public partial class ClassesPageViewModel : ViewModelBase, IEditViewChild {

	private IConfigExportService<GroupConfig> exportService;
	private IConfigService<GroupConfig> configService;

	public ObservableCollection<ClassCardViewModel> Cards { set; get; }

	[ObservableProperty]
	private int activeCardIndex = -1;

	public ClassesPageViewModel(IConfigService<GroupConfig> configService, IConfigExportService<GroupConfig> exportService) {
		this.exportService = exportService;
		this.configService = configService;
		Cards = [];
	}

	public void AddCard() {
		var group = new GroupConfig() {
			Id = Guid.NewGuid(),
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
		await configService.UpdateReferencesAsync();
		List<ClassCardViewModel> cards = await Task.Run(() => {
			var groups = configService.GetData();
			int i = 0;
			return groups
				.Select(g => new ClassCardViewModel(g, i++))
				.ToList();
		});
		Cards.Clear();
		foreach (var card in cards)
			Cards.Add(card);
	}

	protected override async Task OnUnloadAsync() {
		await SetEntitiesFromStateAsync();
	}

	public async Task SetEntitiesFromStateAsync() {
		await Task.Run(() => {
			List<GroupConfig> groups = [];
			var topicService = App.GetRequiredService<IConfigService<TopicConfig>>();
			foreach (var groupCard in Cards) {
				List<BlockConfig> blocks = [];
				foreach (var blockCard in groupCard.Topics) {
					var block = new BlockConfig() {
						Id = blockCard.Config.Id,
						Count = int.Parse(blockCard.Count),
						Topic = topicService.GetById(blockCard.Config.Topic.Id) ?? TopicConfig.Default
					};
					blocks.Add(block);
				}
				var group = new GroupConfig() {
					Id = groupCard.Config.Id,
					AgeGroupName = groupCard.Header,
					SchoolWeeks = groupCard.Config.SchoolWeeks,
					TeacherAbbreviation = groupCard.Config.TeacherAbbreviation,
					Name = groupCard.Header,
					Blocks = blocks
				};
				groups.Add(group);
			}
			configService.SetData(groups);
		});
	}
}