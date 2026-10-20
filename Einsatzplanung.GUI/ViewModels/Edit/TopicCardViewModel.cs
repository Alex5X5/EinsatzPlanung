namespace Einsatzplanung.GUI.ViewModels.Edit;

using Avalonia.Media;

using CommunityToolkit.Mvvm.ComponentModel;

using Einsatzplanung.Types.Models.Configuration;

public partial class TopicCardViewModel : ViewModelBase {

	[ObservableProperty]
	private string header;

	[ObservableProperty]
	private Color selectedColor = Colors.DarkRed;

	public TopicConfig Topic { set; get; }

	public TopicCardViewModel(TopicConfig topic) {
		Topic = topic;
		Header = Topic.Name;
		SelectedColor = Color.Parse(Topic.Color);
	}
}
