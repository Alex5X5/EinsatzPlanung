namespace Einsatzplanung.GUI.ViewModels.Edit;

using CommunityToolkit.Mvvm.Input;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;

using Einsatzplanung.Types.Models;

public partial class ClassCardViewModel : ViewModelBase {
	
	public string Header { get; }
	public ObservableCollection<TopicCardViewModel> Topics { get; }

	public ClassCardViewModel(string header, List<Block> topics) {
		Header = header;
		Topics = new(topics.Select(x => new TopicCardViewModel(x.Name, AddTopicCommand)));
	}

	[RelayCommand]
	private void AddTopic() {
		Topics.Add(new TopicCardViewModel("Neues Thema", AddTopicCommand));
	}
}