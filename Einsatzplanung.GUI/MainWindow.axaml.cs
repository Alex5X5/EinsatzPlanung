using Avalonia.Controls;
using Avalonia.Input;

namespace Einsatzplanung.GUI {
	public partial class MainWindow : Window {
		private Border? _activeCardBorder;

		public MainWindow() {
			InitializeComponent();
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
}