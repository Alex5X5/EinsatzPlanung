using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using DocumentFormat.OpenXml.Spreadsheet;

using System;

namespace Einsatzplanung.GUI.ViewModels.PopUp {
	public partial class ConfirmDialogViewModel : ViewModelBase {
		
		
		public string Header { get; }
		
		public string Message { get; }

		public ConfirmDialogViewModel(string header, string message) {
			Header = header;
			Message = message;
		}
		
		public event Action<bool>? CloseRequested;

		[RelayCommand] 
		private void Yes() => CloseRequested?.Invoke(true);
		
		[RelayCommand] 
		private void No()  => CloseRequested?.Invoke(false);
		
	}
}