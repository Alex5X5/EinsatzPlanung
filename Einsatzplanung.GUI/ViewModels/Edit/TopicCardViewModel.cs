namespace Einsatzplanung.GUI.ViewModels.Edit;

using System;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

public partial class TopicCardViewModel : ViewModelBase {
	
	[ObservableProperty]
	private string topic;

	[ObservableProperty]
	private string count;

	public IRelayCommand AddCommand { init; get; }

    public TopicCardViewModel(string topic, int count, IRelayCommand addCommand) {
        this.topic = topic;
		this.count = Convert.ToString(count);
		AddCommand = addCommand;
    }
}
