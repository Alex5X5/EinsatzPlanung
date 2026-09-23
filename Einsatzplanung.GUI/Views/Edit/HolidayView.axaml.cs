using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;

using Einsatzplanung.GUI.ViewModels.Edit;
using Einsatzplanung.Types.Models;

using Microsoft.Extensions.DependencyInjection;

namespace Einsatzplanung.GUI.Views.Edit;

	public partial class HolidayView : UserControl {
		private Border? _activeCardBorder;
	public HolidayView() {
		InitializeComponent();
		DataContext = App.Current.Services.GetRequiredService<HolidayViewModel>();
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
