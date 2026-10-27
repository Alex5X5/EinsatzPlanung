namespace Einsatzplanung.GUI.Views.Edit;

using System;
using System.Linq;

using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;

using Einsatzplanung.GUI.ViewModels.Edit;


public partial class AgeGroupPageView : ViewBase<AgeGroupPageViewModel> {

	public AgeGroupPageView() : base() {
		InitializeComponent();
	}

	private void OnHeaderDoubleTapped(object? sender, TappedEventArgs e) {
		if (sender is not Control { DataContext: AgeGroupCardViewModel vm } label)
			return;

		vm.IsEditing = true;

		Dispatcher.UIThread.Post(() => {

			if (label.Parent is not Panel panel)
				return;
			if (panel.Children.OfType<TextBox>().FirstOrDefault() is not { } box)
				return;

			box.Focus();
			box.SelectAll();
		}, DispatcherPriority.Input);
	}

	private void OnHeaderLostFocus(object? sender, RoutedEventArgs e) {
		if (sender is Control { DataContext: AgeGroupCardViewModel vm })
			vm.IsEditing = false;
	}

	private void OnHeaderKeyDown(object? sender, KeyEventArgs e) {
		if ((e.Key != Key.Enter && e.Key != Key.Escape)
			|| sender is not Control { DataContext: AgeGroupCardViewModel vm })
			return;

		vm.IsEditing = false;
		e.Handled = true;
	}

	private void RedirectOrEditButton_OnRedirect(object? sender, RoutedEventArgs e) {
		Console.WriteLine("fired!");
	}
}
