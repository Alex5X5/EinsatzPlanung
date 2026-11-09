namespace Einsatzplanung.GUI.ViewModels.Edit;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Einsatzplanung.GUI.Interfaces;
using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models.Configuration;

public partial class BlockCardViewModel : CardViewModel, IRemovableCardViewModel {

	private IConfigService<TopicConfig> topicService;

	[ObservableProperty]
	private string count;

	public ObservableCollection<string> TopicNames { get; private set; }
	
	public BlockConfig Config { get; private set; }

	public BlockCardViewModel(BlockConfig block, IRelayCommand<object> removeCommand, int index) : base(removeCommand, block.Topic.Name, index) {
		topicService = App.GetRequiredService<IConfigService<TopicConfig>>();

		Config = block;
		Count = Config.Count.ToString();

		Task.Run(async ()=>{
			var topics = await topicService.GetDataAsync();
			TopicNames = new(topics.Select(x => x.Name));
		});

	}
}