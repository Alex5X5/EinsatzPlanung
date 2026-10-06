namespace Einsatzplanung.GUI.Views;

using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;

using CommunityToolkit.Mvvm.Input;

using Einsatzplanung.GUI.CodeGenerators.Attributes;
using Einsatzplanung.GUI.ViewModels;
using Einsatzplanung.GUI.Views.Edit;

using Microsoft.Extensions.DependencyInjection;

using System;

public partial class ViewBase : UserControl {

	[BasicStyledProperty<ClassesView>]
	private IRelayCommand loadCommand;

	[BasicStyledProperty<ClassesView>]
	private IRelayCommand unloadCommand;

	public ViewBase(object context) {
		DataContext = context;
		ViewModelBase vm = (DataContext as ViewModelBase)!;
		Bind(LoadCommandProperty, new Binding(nameof(vm.LoadCommand)) { Source = vm });
		Bind(UnloadCommandProperty, new Binding(nameof(vm.UnloadCommand)) { Source = vm });
		AddHandler(LoadedEvent, OnLoad);
		AddHandler(UnloadedEvent, OnUnload);
	}

	private void OnLoad(object? sender, RoutedEventArgs args) {
		if (LoadCommand?.CanExecute(EventArgs.Empty) ?? false)
			LoadCommand.Execute(EventArgs.Empty);
	}

	private void OnUnload(object? sender, RoutedEventArgs args) {
		if (UnloadCommand?.CanExecute(EventArgs.Empty) ?? false)
			UnloadCommand.Execute(EventArgs.Empty);
	}
}

public class ViewBase<T> : ViewBase where T : ViewModelBase {

	public ViewBase() : base(App.Current.Services.GetRequiredService<T>()) { }

}
