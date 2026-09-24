namespace Einsatzplanung.GUI.ViewModels.Edit;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

public partial class TopicCardViewModel : ViewModelBase {
	
	[ObservableProperty]
	private string topic;

	public IRelayCommand AddCommand { init; get; }

    public TopicCardViewModel(string topic, IRelayCommand addCommand) {
        this.topic = topic;
		AddCommand = addCommand;
    }
}
