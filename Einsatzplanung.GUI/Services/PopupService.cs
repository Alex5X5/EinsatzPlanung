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
		public async Task<bool> ConfirmAsync(string title, string message) 
		{
			var owner = (Application.Current?.ApplicationLifetime
				as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
			if (owner is null)
				return false;

			var vm = new ConfirmPopupViewModel(title, message);
			var window = new ConfirmPopupView {
				DataContext = vm
			};

			vm.CloseRequested += result => window.Close(result);

			return await window.ShowDialog<bool>(owner);
		}

		public void Notify(string title, string message) {
			var owner = (Application.Current?.ApplicationLifetime
				as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
			if (owner is null) 
				return;
			
			var vm = new NotifyPopupViewModel(title, message);
			var window = new NotifyPopupView {
				DataContext = vm
			};

			window.ShowDialog(owner);
		}

		public void Warn(string errorMessage ,string title = "Error", string message = "Ein fehler ist aufgetreten!") {
			var owner = (Application.Current?.ApplicationLifetime
				as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
			if (owner is null) 
				return;
			
			var vm = new NotifyPopupViewModel(title, message, isError: true, errorMessage);
			var window = new NotifyPopupView {
				DataContext = vm
			};

			window.ShowDialog(owner);
		}
		
	}