namespace Einsatzplanung.GUI.ViewModels.Edit;

using Avalonia.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Einsatzplanung.Types.Models.Configuration;

public partial class TopicCardViewModel : CardViewModel {

	[ObservableProperty]
	private Color selectedColor = Colors.DarkRed;

	public TopicConfig Topic { set; get; }

	public TopicCardViewModel(TopicConfig topic, IRelayCommand<object> removeCommand, int index) : base(removeCommand, topic.Name, index) {
		Topic = topic;
		SelectedColor = Color.Parse(Topic.Color);
	}
}
