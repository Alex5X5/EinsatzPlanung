using Einsatzplanung.GUI.Views.PopUp;
using Einsatzplanung.GUI.Windows.PopUp;

namespace Einsatzplanung.GUI.Interfaces {
	public interface IPopupFactory
	{
		NotifyPopupView CreateNotifyPopup(
			string title,
			string message,
			bool isError = false,
			string? errorMessage = ""
		);

		ConfirmPopupView CreateConfirmPopup(
			string title,
			string message
		);
	}