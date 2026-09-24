namespace Einsatzplanung.GUI.Views.Edit;

using Einsatzplanung.GUI.ViewModels.Edit;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;

using Microsoft.Extensions.DependencyInjection;

public partial class ClassesView : UserControl {

	public ClassesView() {
		InitializeComponent();
		DataContext = App.Current.Services.GetRequiredService<ClassesPageViewModel>();
	}
	
}
