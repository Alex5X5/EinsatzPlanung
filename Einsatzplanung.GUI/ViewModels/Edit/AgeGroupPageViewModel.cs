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
	private AgeGroupCardViewModel? activeCard = null;

	public AgeGroupPageViewModel(IConfigService<AgeGroupConfig> configService, IConfigExportService<AgeGroupConfig> exportService, IConfigService<TopicConfig> topicService, IConfigService<TeacherConfig> teacherService) {
		this.exportService = exportService;
		this.configService = configService;
		this.topicService = topicService;
		this.teacherService = teacherService;
		Cards = [];
	}

	[RelayCommand]
	public void AddCard() {
		ActiveCard?.AddGroupCommand?.Execute(null);
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
		if (card is AgeGroupCardViewModel ageCard) {
			Cards.Remove(ageCard);
			if(card == ActiveCard)
				if (Cards.Count > 0)
					ActiveCard = Cards[0];
		}
	}

	[RelayCommand]
	private void RedirectToCard(object card) {
		if (card is AgeGroupCardViewModel ageCard) {
			Console.WriteLine("fired redirect event!");
			ActiveCard?.IsActive = false;
			ActiveCard = ageCard;
			ActiveCard.IsActive = true;
		}
	}

	public async Task ExportCardsAsync(string path, CancellationToken? token = null) {
		await SetEntitiesFromStateAsync(token);
		await exportService.ExportAsync(path, await configService.GetDataAsync());
	}

	protected override async Task OnLoadAsync(CancellationToken token) {
		await configService.UpdateReferencesAsync();
		List<AgeGroupCardViewModel> cards = await Task.Run(async () => {
			int i = 0;
			var ageGroups = await configService.GetDataAsync(token);
			return ageGroups
				.Select(g => new AgeGroupCardViewModel(g, RedirectToCardCommand, RemoveCardCommand, i++))
				.ToList();
		});
		Cards.Clear();
		foreach (var card in cards)
			Cards.Add(card);
		RedirectToCard(Cards[0]);
	}

	protected override async Task OnUnloadAsync(CancellationToken token) {
		await SetEntitiesFromStateAsync(token);
	}

	public async Task SetEntitiesFromStateAsync(CancellationToken? token = null) {
		await Task.Run(
			async () => {
				List<AgeGroupConfig> ageGroups = [];
				foreach (var ageGroupCard in Cards) {

					List<GroupConfig> groups = [];
					foreach (var groupCard in ageGroupCard.Cards) {

						List<BlockConfig> blocks = [];
						foreach (var blockCard in groupCard.Topics) {

							var topicOption = await topicService.TryGetByIdAsync(blockCard.Config.Topic.Id, token);
							if (topicOption.Success) {
								var block = new BlockConfig() {
									Id = blockCard.Config.Id,
									Count = int.TryParse(blockCard.Count, out var count) ? count : blockCard.Config.Count,
									Topic = topicOption.Value!,
								};

								blocks.Add(block);
							}

							if (token?.IsCancellationRequested ?? false)
								break;
						}

						var teacherOption = await teacherService.TryGetByIdAsync(groupCard.Config.Teacher.Id, token);
						if (teacherOption.Success) {
							var group = new GroupConfig() {
								Id = groupCard.Config.Id,
								Name = groupCard.Header,
								Teacher = teacherOption.Value!,
								SchoolWeeks = groupCard.Config.SchoolWeeks,
								Blocks = blocks
							};

							groups.Add(group);
						}


						if (token?.IsCancellationRequested ?? false)
							break;
					}

					var ageGroup = new AgeGroupConfig() {
						Id = ageGroupCard.Config.Id,
						Name = ageGroupCard.Header,
						Groups = groups
					};

					ageGroups.Add(ageGroup);

					if (token?.IsCancellationRequested ?? false)
						break;
				}

				await configService.SetDataAsync(ageGroups);
			});
	}
}