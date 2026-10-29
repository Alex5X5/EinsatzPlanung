namespace Einsatzplanung.GUI.ViewModels.Edit;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models.Configuration;

public partial class AgeGroupCardViewModel : CardViewModel {

	public ObservableCollection<GroupCardViewModel> Cards { get; }

	public AgeGroupConfig Config { init; get; }

	public IRelayCommand<object> RedirectCommand { set; get; }

	[ObservableProperty]
	private bool isActive = false;

	public AgeGroupCardViewModel(AgeGroupConfig ageGroup, IRelayCommand<object> redirectCommand, IRelayCommand<object> removeCommand, int index) : base(removeCommand, ageGroup.Name, index) {
		Config = ageGroup;
		RedirectCommand = redirectCommand;
		int i = 0;
		var models = ageGroup.Groups.Select(x => new GroupCardViewModel(x, RemoveGroupCommand, i++));
		Cards = new(models);
	}

	[RelayCommand]
	private async Task AddGroup() {
		var teachers = await App.GetRequiredService<IConfigService<TeacherConfig>>().GetDataAsync();
		var teacher = teachers.FirstOrDefault(TeacherConfig.Default);
		var group = new GroupConfig() {
			Id = Guid.NewGuid(),
			Name = "",
			Blocks = [],
			SchoolWeeks = [],
			Teacher = teacher,
		};
		Cards.Add(new GroupCardViewModel(group, RemoveGroupCommand, Cards.Count));
	}

	[RelayCommand]
	private void RemoveGroup(object group) {
		if (group is GroupCardViewModel model)
			Cards.Remove(model);
	}
}