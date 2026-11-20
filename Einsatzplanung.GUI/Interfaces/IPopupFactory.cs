using Einsatzplanung.GUI.ViewModels.PopUp;
using Einsatzplanung.GUI.Views.PopUp;
using Einsatzplanung.GUI.Windows.PopUp;

namespace Einsatzplanung.GUI.Interfaces {
	public interface IPopupFactory {
		NotifyPopupView CreateNotifyPopup(
			string title,
			string message,
			bool isError = false,
			string? errorMessage = ""
		);

		(ConfirmPopupViewModel vm, ConfirmPopupView view) CreateConfirmPopup(
			string title,
			string message);
	}
}