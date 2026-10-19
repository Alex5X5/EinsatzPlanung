namespace Einsatzplanung.GUI.ViewModels.Edit;

using System.Linq;
using System.Collections.ObjectModel;

using Avalonia.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Einsatzplanung.GUI.Interfaces;
using Einsatzplanung.Types.Models.Configuration;


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
				if(Color.TryParse(x.Color, out var color))
					return new BlockCardViewModel(x.Name, color, x.Count, RemoveTopicCommand);
				else
					return new BlockCardViewModel(x.Name, x.Count, RemoveTopicCommand);
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