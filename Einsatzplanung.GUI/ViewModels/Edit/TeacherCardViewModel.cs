namespace Einsatzplanung.GUI.ViewModels.Edit;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Einsatzplanung.GUI.Interfaces;
using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Input.Services;
using Einsatzplanung.Types.Models.Configuration;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

public partial class TeacherCardViewModel : CardViewModel {

	[ObservableProperty]
	private string header;
	[ObservableProperty]
	private string abbreviation;
	[ObservableProperty]
	private int weeklyHours;

	public ObservableCollection<SpecializationCardViewModel> Specializations { get; init; }
	public List<string> Topics { get; init; }
	
	public TeacherConfig Config { init; get; }

	public TeacherCardViewModel(TeacherConfig teacher, int index) : base(index) {
		Config = teacher;
		Header = teacher.Name;
		Abbreviation = teacher.Abbreviation;
		WeeklyHours = teacher.WeeklyHours;
		var topics = App.GetRequiredService<IConfigService<TopicConfig>>().GetData();
		Specializations = new(teacher.Specializations.Select(t => new SpecializationCardViewModel(t, RemoveTopicCommand)));
		Topics = topics.Select(x => x.Name).ToList();
	}

	[RelayCommand]
	private void AddTopic() {
		var topics = App.GetRequiredService<IConfigService<TopicConfig>>().GetData();
		Specializations.Add(new SpecializationCardViewModel(topics[0], RemoveTopicCommand));
	}

	[RelayCommand]
	private void RemoveTopic(object topic) {
		if (topic is SpecializationCardViewModel topicToRemove)
			Specializations.Remove(topicToRemove);
	}
}