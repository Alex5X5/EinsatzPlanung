using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using DocumentFormat.OpenXml.Spreadsheet;

using System;

namespace Einsatzplanung.GUI.ViewModels.PopUp {
	public partial class ConfirmDialogViewModel : ViewModelBase {
		
		
		public string Header { get; }
		
		public string Message { get; }
		
		public string FirstButtonText { get; }
		
		public string SecondButtonText { get; }

		/// <summary>
		/// Creates a view model for a confirmation dialog with two buttons.
		/// </summary>
		/// <param name="header">The title shown at the top of the dialog.</param>
		/// <param name="message">The message text displayed in the dialog body.</param>
		/// <param name="f">Text of the first button (usually the confirm action). Defaults to "Ja".</param>
		/// <param name="s">Text of the second button (usually the cancel action). Defaults to "Nein".</param>
		public ConfirmDialogViewModel(string header, string message, string f = "Ja", string s = "Nein") {
			Header = header;
			Message = message;
			FirstButtonText = f;
			SecondButtonText = s;
		}
		
		public event Action<bool>? CloseRequested;

		[RelayCommand] 
		private void Yes() => CloseRequested?.Invoke(true);
		
		[RelayCommand] 
		private void No()  => CloseRequested?.Invoke(false);
		
	}
}