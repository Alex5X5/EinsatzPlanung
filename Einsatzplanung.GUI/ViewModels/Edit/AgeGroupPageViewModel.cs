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
	private IConfigService<TeacherConfig> teacherService;

	public ObservableCollection<AgeGroupCardViewModel> Cards { set; get; }

	[ObservableProperty]
	private int activeAgeGroupCardIndex = -1;

	[ObservableProperty]
	private int activeGroupCardIndex = -1;

	[ObservableProperty]
	private AgeGroupCardViewModel activeCard;

	public AgeGroupPageViewModel(IConfigService<AgeGroupConfig> configService, IConfigExportService<AgeGroupConfig> exportService, IConfigService<TopicConfig> topicService) {
		this.exportService = exportService;
		this.configService = configService;
		this.topicService = topicService;
		this.teacherService = teacherService;
		Cards = [];
	}

	[RelayCommand]
	public void AddCard() {
		ActiveCard.AddGroupCommand.Execute(null);
	}

	[RelayCommand]
	public void AddAgeGroup() {
		var group = new AgeGroupConfig() {
			Id = Guid.NewGuid(),
			Name = "",
			Groups = []
		};

		Cards.Add(new AgeGroupCardViewModel(group, RedirectToCardCommand, RemoveCardCommand, Cards.Count));
	}

	[RelayCommand]
	private void RemoveCard(object card) {
		if (card is AgeGroupCardViewModel ageCard)
			Cards.Remove(ageCard);
	}

	[RelayCommand]
	private void RedirectToCard(object card) {
		if (card is AgeGroupCardViewModel ageCard) {
			Console.WriteLine("fired redirect event!");
			ActiveCard = ageCard;
		}
	}

	public async Task ExportCardsAsync(string path) {
		await SetEntitiesFromStateAsync();
		await exportService.ExportAsync(path, configService.GetData());
	}

	protected override async Task OnLoadAsync() {
		//await configService.UpdateReferencesAsync();
		List<AgeGroupCardViewModel> cards = await Task.Run(() => {
			int i = 0;
			return configService
				.GetData()
				.Select(g => new AgeGroupCardViewModel(g, RedirectToCardCommand, RemoveCardCommand, i++))
				.ToList();
		});
		Cards.Clear();
		foreach (var card in cards)
			Cards.Add(card);
		if(Cards.Count > 0)
			ActiveCard = Cards[0];
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
						Name = ageGroupCard.Header,
						Groups = ageGroupCard.Cards
							.Select(groupCard =>
								new GroupConfig() {
									Id = groupCard.Config.Id,
									Name = groupCard.Header,
									SchoolWeeks = groupCard.Config.SchoolWeeks,
									Teacher = groupCard.Config.Teacher,
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