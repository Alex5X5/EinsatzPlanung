namespace Einsatzplanung.GUI.ViewModels.Edit;

using Avalonia.Media;

using CommunityToolkit.Mvvm.ComponentModel;

using Einsatzplanung.Types.Models.Configuration;

public partial class TopicCardViewModel : ViewModelBase {
	
	[ObservableProperty]
	private string header;
	    
	[ObservableProperty]
	private Color selectedColor = Colors.DarkRed;

	public TopicCardViewModel(TopicConfig topic) : this(topic.Name, Color.Parse(topic.Color)) { }

	public TopicCardViewModel(string header, Color selectedColor) {
		Header = header;
		SelectedColor = selectedColor;
	}
}
