namespace Einsatzplanung.GUI.ViewModels.Edit;

using Einsatzplanung.GUI.ViewModels;

using System.Collections.ObjectModel;


public class ClassCardViewModel
{
	public string Header { get; }

	public ClassCardViewModel(string header)
	{
		Header = header;
	}
}