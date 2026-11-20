using Avalonia.Controls;

using Einsatzplanung.GUI.Interfaces;
using Einsatzplanung.GUI.Windows.PopUp;

using System;

namespace Einsatzplanung.GUI.Services;

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;

using Einsatzplanung.GUI.ViewModels.PopUp;
using Einsatzplanung.GUI.Views;
using Einsatzplanung.GUI.Views.PopUp;

using System.Threading.Tasks;

public class PopupService : IPopupService {

	private readonly IPopupFactory _popupFactory;

	public PopupService(IPopupFactory popupFactory) {
		_popupFactory = popupFactory;
	}

	public async Task<bool> ConfirmAsync(string title, string message) {
		var owner = (Application.Current?.ApplicationLifetime
			as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
		if (owner is null)
			return false;

		var vm = new ConfirmPopupViewModel(title, message);
		var window = new ConfirmPopupView { DataContext = vm };

		vm.CloseRequested += result => window.Close(result);

		return await window.ShowDialog<bool>(owner);
	}

	public void Notify(string title, string message) {
		Window owner = GetOwner();

		NotifyPopupView window = _popupFactory.CreateNotifyPopup(title, message);

		window.ShowDialog(owner);
	}

	public void Warn(string errorMessage, string title = "Error", string message = "Ein fehler ist aufgetreten!") {
		Window owner = GetOwner();

		NotifyPopupView window = _popupFactory.CreateNotifyPopup(title, message, true, errorMessage);

		window.ShowDialog(owner);
	}

	private static Window GetOwner() {
		Window? owner = (Application.Current?.ApplicationLifetime
			as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
		return owner ?? throw new NullReferenceException();
	}
}