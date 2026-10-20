namespace Einsatzplanung.GUI.ViewModels.Edit;

using System.Collections.Generic;
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
	private readonly IReadOnlyList<string> topicNames;

	public TeacherCardViewModel(TeacherConfig teacher, int index, IReadOnlyList<string> topicNames) : base(index) {
		this.topicNames = topicNames;
		Header = teacher.Name;
		Abbreviation = teacher.Abbreviation;
		WeeklyHours = teacher.WeeklyHours;
		Topics = new(teacher.Specializations.Select(t => new SpecializationCardViewModel(t.Name, topicNames, RemoveTopicCommand)));
	}

	[RelayCommand]
	private void AddTopic() {
		Topics.Add(new SpecializationCardViewModel(topicNames[0], topicNames, RemoveTopicCommand));
	}

	[RelayCommand]
	private void RemoveTopic(object topic) {
		if (topic is SpecializationCardViewModel topicToRemove)
			Topics.Remove(topicToRemove);
	}
}