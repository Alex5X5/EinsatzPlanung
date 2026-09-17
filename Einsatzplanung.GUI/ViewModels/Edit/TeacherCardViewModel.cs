namespace Einsatzplanung.GUI.ViewModels.Edit;
using CommunityToolkit.Mvvm.Input;

using Microsoft.Extensions.DependencyInjection;

using System.Collections.ObjectModel;

using Einsatzplanung.GUI;
using Einsatzplanung.Types.Models;
using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Generation.Interfaces;


public class TeacherCardViewModel : ViewModelBase
{

	public string Header { get; }
	public ObservableCollection<TeacherTopicCardViewModel> Topics { get; }

	public TeacherCardViewModel(string header)
	{
		Header = header;
		Topics =
			[
				new TeacherTopicCardViewModel("Grundlagen"),
				new TeacherTopicCardViewModel("Programmierung"),
				new TeacherTopicCardViewModel("Projektmanagement"),
				new TeacherTopicCardViewModel("Netzwerke")
			];
	}
}