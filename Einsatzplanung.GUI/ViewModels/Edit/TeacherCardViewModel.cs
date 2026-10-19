namespace Einsatzplanung.GUI.ViewModels.Edit;

using CommunityToolkit.Mvvm.Input;
using System.Linq;
using System.Collections.ObjectModel;

using Einsatzplanung.GUI.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using Einsatzplanung.Types.Models.Configuration;

public partial class TeacherCardViewModel : CardViewModel {

	[ObservableProperty]
	private string header;
	[ObservableProperty]
	private string abbreviation;
	[ObservableProperty]
	private int weeklyHours;
	public ObservableCollection<SpecializationCardViewModel> Topics { get; }

	public TeacherCardViewModel(TeacherConfig teacher, int index) : base(index) {
		Header = teacher.Name;
		Abbreviation = teacher.Abbreviation;
		WeeklyHours = teacher.WeeklyHours;
		Topics = new(teacher.Specializations.Select(t => new SpecializationCardViewModel(t.Name, RemoveTopicCommand)));
	}

	[RelayCommand]
	private void AddTopic() {
		Topics.Add(new SpecializationCardViewModel("Neue Spezialisierung", RemoveTopicCommand));
	}

	[RelayCommand]
	private void RemoveTopic(object topic) {
		if(topic is IRemovableCardViewModel model)
			if(Topics.FirstOrDefault(t => t.Header == model.Header) is SpecializationCardViewModel topicToRemove)
				Topics.Remove(topicToRemove);
	}
}