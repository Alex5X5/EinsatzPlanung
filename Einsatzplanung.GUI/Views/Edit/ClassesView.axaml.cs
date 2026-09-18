namespace Einsatzplanung.GUI.Views.Edit;

using Einsatzplanung.GUI.ViewModels.Edit;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;

using Microsoft.Extensions.DependencyInjection;

public partial class ClassesView : UserControl {
	
	private Border? _activeCardBorder;
	public ClassesView() {
		InitializeComponent();
		DataContext = App.Current.Services.GetRequiredService<ClassesPageViewModel>();
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
