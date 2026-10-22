namespace Einsatzplanung.GUI.ViewModels.Edit;

using Avalonia.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Einsatzplanung.Types.Models.Configuration;

public partial class TopicCardViewModel : CardViewModel {

	[ObservableProperty]
	private string header;

	[ObservableProperty]
	private Color selectedColor = Colors.DarkRed;

	public TopicConfig Topic { set; get; }

	public TopicCardViewModel(TopicConfig topic, IRelayCommand<object> removeCommand, int index) : base(removeCommand, index) {
		Topic = topic;
		Header = Topic.Name;
		SelectedColor = Color.Parse(Topic.Color);
	}
}
