namespace Einsatzplanung.GUI.ViewModels.Edit;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models.Configuration;

public partial class GroupCardViewModel : CardViewModel {

	private IConfigService<TeacherConfig> teacherService;

	[ObservableProperty]
	private string header;
	
	partial void OnHeaderChanged(string value) {
		Config = new GroupConfig() {
			Id = Config.Id,
			Name = value,
			Blocks = Config.Blocks,
			SchoolWeeks = Config.SchoolWeeks,
			Teacher = Config.Teacher
		};
	}

	[ObservableProperty]
	private string teacherAbbreviation = "";

	partial void OnTeacherAbbreviationChanged(string value) {
		Config = new GroupConfig() {
			Id = Config.Id,
			Name = Config.Name,
			Blocks = Config.Blocks,
			SchoolWeeks = Config.SchoolWeeks,
			Teacher = teacherService.GetByAbbreviation(value) ?? TeacherConfig.Default
		};
	}
	
	public IReadOnlyList<string> Abbreviations { get; }

	public ObservableCollection<BlockCardViewModel> Topics { get; }
	
	public GroupConfig Config { set; get; }

	public GroupCardViewModel(GroupConfig group, IRelayCommand<object> removeCommand, int index) : base(removeCommand, index) {
		teacherService = App.GetRequiredService<IConfigService<TeacherConfig>>();

		Config = group;
		Header = group.Name;
		TeacherAbbreviation = Config.Teacher.Abbreviation;

		var teachers = teacherService.GetData();
		Abbreviations = teachers.Select(x => x.Abbreviation).ToList();

		int i = 0;
		var models = group.Blocks.Select(x => new BlockCardViewModel(x, RemoveTopicCommand, i++));
		Topics = new(models);
	}

	[RelayCommand]
	private void AddTopic() {
		var topic = App.GetRequiredService<IConfigService<TopicConfig>>().GetData().FirstOrDefault(TopicConfig.Default);
		var block = new BlockConfig() {
			Id = Guid.NewGuid(),
			Count = 1,
			Topic = topic
		};
		Topics.Add(new BlockCardViewModel(block, RemoveTopicCommand, Topics.Count));
	}

	[RelayCommand]
	private void RemoveTopic(object topic) {
		if (topic is BlockCardViewModel topicToRemove)
			Topics.Remove(topicToRemove);
	}
}