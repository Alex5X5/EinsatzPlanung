namespace Einsatzplanung.GUI.ViewModels.Edit;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models.Configuration;

public partial class TeacherCardViewModel : CardViewModel {

	[ObservableProperty]
	private string abbreviation;
	[ObservableProperty]
	private int weeklyHours;

	public ObservableCollection<SpecializationCardViewModel> Specializations { get; init; }
	public List<string> Topics { get; init; }
	
	public TeacherConfig Config { init; get; }

	public TeacherCardViewModel(TeacherConfig teacher, IRelayCommand<object> removeCommand, int index) : base(removeCommand, teacher.Name, index) {
		Config = teacher;
		Abbreviation = teacher.Abbreviation;
		WeeklyHours = teacher.WeeklyHours;
		var topics = App.GetRequiredService<IConfigService<TopicConfig>>().GetDataAsync().Result;
		Specializations = new(teacher.Specializations.Select(t => new SpecializationCardViewModel(t, RemoveTopicCommand)));
		Topics = topics.Select(x => x.Name).ToList();
	}

	[RelayCommand]
	private async Task AddTopic() {
		var topics = await App.GetRequiredService<IConfigService<TopicConfig>>().GetDataAsync();
		Specializations.Add(new SpecializationCardViewModel(topics[0], RemoveTopicCommand));
	}

	[RelayCommand]
	private void RemoveTopic(object topic) {
		if (topic is SpecializationCardViewModel topicToRemove)
			Specializations.Remove(topicToRemove);
	}
}