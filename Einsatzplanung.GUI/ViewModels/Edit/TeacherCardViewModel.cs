namespace Einsatzplanung.GUI.ViewModels.Edit;

using CommunityToolkit.Mvvm.Input;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;

using Einsatzplanung.Types.Models;
using Einsatzplanung.GUI.Interfaces;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Einsatzplanung.Types.Models.Configuration;
using Avalonia.Controls.Primitives;

public partial class TeacherCardViewModel : ViewModelBase {

	[ObservableProperty]
	private string header;
	[ObservableProperty]
	private string abbreviation;
	[ObservableProperty]
	private int weeklyHours;
	public ObservableCollection<TopicCardViewModel> Topics { get; }

	public TeacherCardViewModel(TeacherConfig teacher) {
		Header = teacher.Name;
		Abbreviation = teacher.Abbreviation;
		WeeklyHours = teacher.WeeklyHours;
		Topics = new(teacher.Specializations.Select(t => new TopicCardViewModel(t.Name, 0, RemoveTopicCommand)));
	}

	[RelayCommand]
	private void AddTopic() {
		Topics.Add(new TopicCardViewModel("Neue Spezialisierung", 0, RemoveTopicCommand));
	}

	[RelayCommand]
	private void RemoveTopic(object topic) {
		if(topic is IRemovableCardViewModel model)
			if(Topics.FirstOrDefault(t => t.Header == model.Header) is TopicCardViewModel topicToRemove)
				Topics.Remove(topicToRemove);
	}
}