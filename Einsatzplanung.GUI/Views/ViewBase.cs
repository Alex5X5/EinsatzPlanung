using Avalonia.Controls;
using Avalonia.Data;

using CommunityToolkit.Mvvm.Input;

using Einsatzplanung.GUI.CodeGenerators.Attributes;
using Einsatzplanung.GUI.ViewModels;
using Einsatzplanung.GUI.Views.Edit;

namespace Einsatzplanung.GUI.Views;

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
