namespace Einsatzplanung.GUI.Services;

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;

using Einsatzplanung.GUI.ViewModels.PopUp;
using Einsatzplanung.GUI.Views;
using Einsatzplanung.GUI.Views.PopUp;

using System.Threading.Tasks;
	public class DialogService : IDialogService {
		public async Task<bool> ConfirmAsync(string title, string message) 
		{
			var owner = (Application.Current?.ApplicationLifetime
				as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
			if (owner is null)
				return false;

			var vm = new ConfirmDialogViewModel(title, message);
			var window = new ConfirmDialogView {
				DataContext = vm
			};

			vm.CloseRequested += result => window.Close(result);

			return await window.ShowDialog<bool>(owner);
		}
	}