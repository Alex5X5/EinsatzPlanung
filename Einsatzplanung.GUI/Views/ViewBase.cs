namespace Einsatzplanung.GUI.Views;

using Avalonia.Controls;
using Avalonia.Data;

using Microsoft.Extensions.DependencyInjection;

using CommunityToolkit.Mvvm.Input;

using Einsatzplanung.GUI.CodeGenerators.Attributes;
using Einsatzplanung.GUI.ViewModels;
using Einsatzplanung.GUI.Views.Edit;

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
	}
}

public class ViewBase<T> : ViewBase where T : ViewModelBase {

	public ViewBase() : base(App.Current.Services.GetRequiredService<T>()) { }

}
