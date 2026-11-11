namespace Einsatzplanung.GUI.ViewModels.PopUp;
using CommunityToolkit.Mvvm.Input;

public partial class NotifyPopupViewModel : ViewModelBase {

	public NotifyPopupViewModel(string header, string message, bool isError = false, string errorMessage = "") {
		Header = header;
		Message = message;
		IsError = isError;
		ErrorMessage = errorMessage;
	}
	
	public string Header { get; set; }
	public string Message { get; set; }
	public bool IsError { get; set; }
	public string ErrorMessage { get; set; }
	
}
