namespace Einsatzplanung.GUI.ViewModels.Edit;

using System.Linq;
using System.Collections.ObjectModel;

using Avalonia.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Einsatzplanung.GUI.Interfaces;
using Einsatzplanung.Types.Models.Configuration;

using Microsoft.Extensions.DependencyInjection;
using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Input.Services;
using Einsatzplanung.Types.Models;

public partial class ClassCardViewModel : CardViewModel {
	
	[ObservableProperty]
	private string header;
	public ObservableCollection<BlockCardViewModel> Topics { get; }
	public GroupConfig Config { init; get; }

	public ClassCardViewModel(GroupConfig group, int index) : base(index) {
		Config = group;
		Header = group.Name;
		var models = group.Blocks.Select(
			x => {
				var topic = App.Current.Services.GetRequiredService<IEntityService<Topic>>().GetTopicByName(x.Topic);
				return new BlockCardViewModel(topic.Name, x.Count, RemoveTopicCommand);
			});
		Topics = new(models);
	}

	[RelayCommand]
	private void AddTopic() {
		Topics.Add(new BlockCardViewModel("Neues Thema", 1, RemoveTopicCommand));
	}

	[RelayCommand]
	private void RemoveTopic(object topic) {
		if (topic is IRemovableCardViewModel model)
			if (Topics.FirstOrDefault(t => t.Header == model.Header) is BlockCardViewModel topicToRemove)
				Topics.Remove(topicToRemove);
	}
}