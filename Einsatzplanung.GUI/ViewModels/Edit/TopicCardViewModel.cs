namespace Einsatzplanung.GUI.ViewModels.Edit;

using Avalonia.Media;

using CommunityToolkit.Mvvm.ComponentModel;

using Einsatzplanung.Types.Models;

public partial class TopicCardViewModel : ViewModelBase {
	
	[ObservableProperty]
	private string header;
	    
	[ObservableProperty]
	private Color selectedColor = Colors.DarkRed;

	public TopicCardViewModel(Topic topic) : this(topic.Name, Color.Parse("#FFFFFFFFF")) { }

	public TopicCardViewModel(string header, Color selectedColor) {
		Header = header;
		SelectedColor = selectedColor;
	}
}
