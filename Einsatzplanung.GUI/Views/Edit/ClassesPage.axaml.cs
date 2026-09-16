using Einsatzplanung.GUI.ViewModels.Edit;

namespace Einsatzplanung.GUI.Views.Edit;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;

public partial class ClassesPage : UserControl {
	
	private Border? _activeCardBorder;
	public ClassesPage() {
		InitializeComponent();
		
		DataContext = new ClassesPageViewModel();
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
