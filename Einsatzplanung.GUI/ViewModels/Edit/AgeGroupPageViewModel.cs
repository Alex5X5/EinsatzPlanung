namespace Einsatzplanung.GUI.ViewModels.Edit;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Einsatzplanung.Excel.Interfaces;
using Einsatzplanung.GUI.Interfaces;
using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models;
using Einsatzplanung.Types.Models.Configuration;

public partial class AgeGroupPageViewModel : ViewModelBase, IEditViewChild {

	private IConfigExportService<AgeGroupConfig> exportService;
	private IConfigService<AgeGroupConfig> configService;
	private IConfigService<TopicConfig> topicService;

	public ObservableCollection<AgeGroupCardViewModel> Cards { set; get; }

	[ObservableProperty]
	private int activeAgeGroupCardIndex = -1;

	[ObservableProperty]
	private int activeGroupCardIndex = -1;

	public AgeGroupPageViewModel(IConfigService<AgeGroupConfig> configService, IConfigExportService<AgeGroupConfig> exportService, IConfigService<TopicConfig> topicService) {
		this.exportService = exportService;
		this.configService = configService;
		this.topicService = topicService;
		Cards = [];
	}

	[RelayCommand]
	public void AddCard() {
		var group = new AgeGroupConfig() {
			Id = Guid.NewGuid(),
			Name = "",
			Groups = []
		};

		Cards.Add(new AgeGroupCardViewModel(group, RemoveCardCommand, Cards.Count));
	}

	[RelayCommand]
	private void RemoveCard(object card) {
		if (card is AgeGroupCardViewModel ageCard)
			Cards.Remove(ageCard);
	}

	public async Task ExportCardsAsync(string path) {
		await SetEntitiesFromStateAsync();
		await exportService.ExportAsync(path, configService.GetData());
	}

	protected override async Task OnLoadAsync() {
		await configService.UpdateReferencesAsync();
		List<AgeGroupCardViewModel> cards = await Task.Run(() => {
			var groups = configService.GetData();
			int i = 0;
			return groups
				.Select(g => new AgeGroupCardViewModel(g, RemoveCardCommand, i++))
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
			List<AgeGroupConfig> ageGroups = Cards
				.Select(ageGroupCard =>
					new AgeGroupConfig() {
						Id = ageGroupCard.Config.Id,
						Name = ageGroupCard.Config.Name,
						Groups = ageGroupCard.Cards
							.Select(groupCard =>
								new GroupConfig() {
									Id = groupCard.Config.Id,
									Name = groupCard.Config.Name,
									AgeGroupName = groupCard.Config.Name,
									TeacherAbbreviation = groupCard.Config.TeacherAbbreviation,
									SchoolWeeks = groupCard.Config.SchoolWeeks,
									Blocks = groupCard.Topics
										.Select(blockCard =>
											new BlockConfig() {
												Id = blockCard.Config.Id,
												Count = int.Parse(blockCard.Count),
												Topic = topicService.GetByName(blockCard.Header) ?? TopicConfig.Default
											})
										.ToList()

								})
							.ToList()
					})
				.ToList();
			configService.SetData(ageGroups);
		});
	}
}