namespace Einsatzplanung.GUI.ViewModels.Edit;
using CommunityToolkit.Mvvm.Input;

using Microsoft.Extensions.DependencyInjection;

using System.Collections.ObjectModel;

using Einsatzplanung.GUI;
using Einsatzplanung.Types.Models;
using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Generation.Interfaces;


public class ClassesPageViewModel : ViewModelBase
{

	public ObservableCollection<ClassCardViewModel> Cards { get; }

	public ClassesPageViewModel()
	{
		Cards =
		[
			new ClassCardViewModel("FI24"),
			new ClassCardViewModel("FI25"),
			new ClassCardViewModel("FI26")
		];
	}
}