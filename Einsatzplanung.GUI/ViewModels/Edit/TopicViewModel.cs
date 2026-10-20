namespace Einsatzplanung.GUI.ViewModels.Edit;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

using Einsatzplanung.Excel.Interfaces;
using Einsatzplanung.GUI.Interfaces;
using Einsatzplanung.GUI.Helpers;
using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models.Configuration;

public partial class TopicViewModel : ViewModelBase, IEditViewChild {

	private IConfigExportService<TopicConfig> exportService;
	private IConfigService<TopicConfig> configService;

	public string Header { get; }

	[ObservableProperty]
	private ObservableCollection<TopicCardViewModel> cards;

	[ObservableProperty]
	private int activeCardIndex = -1;

	public TopicViewModel(IConfigService<TopicConfig> configService, IConfigExportService<TopicConfig> exportService) {
		this.exportService = exportService;
		this.configService = configService;
		Cards = [];
	}

	public void AddCard() {
		var topic = configService.GetData().FirstOrDefault(TopicConfig.Default);
		Cards.Add(new TopicCardViewModel(topic));
	}

	protected override async Task OnLoadAsync() {
		List<TopicCardViewModel> cards = await Task.Run(
			() => configService
				.GetData()
				.Select(t => new TopicCardViewModel(t))
				.ToList());
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
			List<TopicConfig> topics = [];
			foreach (var card in Cards) {
				var topic = card.Topic;
				topic.Name = card.Header;
				topic.Color = card.SelectedColor.ToRgbHex();
				topics.Add(topic);
			}
			configService.SetData(topics);
		});
	}
}