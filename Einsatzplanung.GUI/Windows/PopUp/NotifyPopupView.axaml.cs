using Avalonia.Data;

namespace Einsatzplanung.GUI.Windows.PopUp;

using Avalonia.Controls;

using CommunityToolkit.Mvvm.Input;

using Einsatzplanung.GUI.CodeGenerators.Attributes;

public partial class NotifyPopupView : Window {

	[BasicStyledProperty]
	private IRelayCommand closeWindowCommand;
	
	public NotifyPopupView() {
		InitializeComponent();
		CloseWindowCommand = new RelayCommand(Close);
	}
}