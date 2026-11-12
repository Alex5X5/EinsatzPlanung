namespace Einsatzplanung.GUI.ViewModels.Edit;

using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models.Configuration;

public partial class GroupCardViewModel : CardViewModel {

	private IConfigService<TeacherConfig> teacherService;

	[ObservableProperty]
	private string teacherAbbreviation = "";
	
	public ObservableCollection<string> Abbreviations { get; private set; }

	public ObservableCollection<BlockCardViewModel> Topics { get; }
	
	public GroupConfig Config { set; get; }

	public GroupCardViewModel(GroupConfig group, IRelayCommand<object> removeCommand, int index) : base(removeCommand, group.Name, index) {
		teacherService = App.GetRequiredService<IConfigService<TeacherConfig>>();

		Config = group;
		TeacherAbbreviation = Config.Teacher.Abbreviation;

		Task.Run(async () => {
			var teachers = await teacherService.GetDataAsync();
			Abbreviations = new(teachers.Select(x => x.Abbreviation));
		});
		
		int i = 0;
		var models = group.Blocks.Select(x => new BlockCardViewModel(x, RemoveTopicCommand, i++));
		Topics = new(models);
	}

	[RelayCommand]
	private async Task AddTopic() {
		var topics = await App.GetRequiredService<IConfigService<TopicConfig>>().GetDataAsync();
		var topic = topics.FirstOrDefault(TopicConfig.Default);
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