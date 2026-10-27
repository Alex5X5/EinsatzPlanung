namespace Einsatzplanung.GUI.ViewModels.Edit;

using System;
using System.Linq;
using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models.Configuration;


public partial class AgeGroupCardViewModel : CardViewModel {

	[ObservableProperty]
	private string header;
	
	[ObservableProperty]
	private bool isEditing;

	public ObservableCollection<GroupCardViewModel> Cards { get; }

	public AgeGroupConfig Config { init; get; }

	public IRelayCommand<object> RedirectCommand { set; get; }

	public AgeGroupCardViewModel(AgeGroupConfig ageGroup, IRelayCommand<object> redirectCommand, IRelayCommand<object> removeCommand, int index) : base(removeCommand, index) {
		Config = ageGroup;
		Header = ageGroup.Name;
		int i = 0;
		var models = ageGroup.Groups.Select(x => new GroupCardViewModel(x, RemoveGroupCommand, i++));
		Cards = new(models);
	}

	[RelayCommand]
	private void AddGroup() {
		var teacher = App.GetRequiredService<IConfigService<TeacherConfig>>().GetData().FirstOrDefault(TeacherConfig.Default);
		var group = new GroupConfig() {
			Id = Guid.NewGuid(),
			Name = "",
			AgeGroupName = Config.Name,
			Blocks = [],
			SchoolWeeks = [],
			TeacherAbbreviation =""
		};
		Cards.Add(new GroupCardViewModel(group, RemoveGroupCommand, Cards.Count));
	}

	[RelayCommand]
	private void RemoveGroup(object group) {
		if (group is GroupCardViewModel model)
			Cards.Remove(model);
	}
}