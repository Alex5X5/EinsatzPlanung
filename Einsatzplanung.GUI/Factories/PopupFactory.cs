using Einsatzplanung.GUI.ViewModels.PopUp;
using Einsatzplanung.GUI.Views.PopUp;
using Einsatzplanung.GUI.Windows.PopUp;

namespace Einsatzplanung.GUI.Factories;

using Einsatzplanung.GUI.Interfaces;

public class PopupFactory : IPopupFactory {
	
	public NotifyPopupView CreateNotifyPopup(string title, string message, bool isError = false, string errorMessage = "") {

		var vm = new NotifyPopupViewModel(title, message, isError, errorMessage);
		return new NotifyPopupView {
			DataContext = vm
		};
	}
	
	public ConfirmPopupView CreateConfirmPopup(
		string title,
		string message)
	{
		var vm = new ConfirmPopupViewModel(title, message);
		return new ConfirmPopupView
		{
			DataContext = vm
		};
	}
	
}
