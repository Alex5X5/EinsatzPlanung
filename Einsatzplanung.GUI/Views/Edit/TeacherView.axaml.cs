using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

using Einsatzplanung.Types.Models;
using Einsatzplanung.GUI.ViewModels.Edit;
using Avalonia.Input;

namespace Einsatzplanung.GUI.Views.Edit;

	public partial class TeacherView : UserControl {
		private Border? _activeCardBorder;
	public TeacherView() {
		InitializeComponent();
		
		DataContext = new TeacherPageViewModel();
	}
	
	private void CardBorder_OnPointerEntered(object? sender, PointerEventArgs e) {
		if (sender is not Border currentBorder)
			return;

		if (_activeCardBorder == currentBorder)
			return;

		_activeCardBorder?.Classes.Remove("active");

		currentBorder.Classes.Add("active");
		_activeCardBorder = currentBorder;
	}
}
