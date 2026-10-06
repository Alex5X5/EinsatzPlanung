namespace Einsatzplanung.GUI.Views.Edit;

using Avalonia.Markup.Xaml;
using Einsatzplanung.GUI.ViewModels.Edit;

using Microsoft.Extensions.DependencyInjection;

public partial class ClassesView : ViewBase {

	public ClassesView() : base(App.Current.Services.GetRequiredService<ClassesPageViewModel>()) {
		InitializeComponent();
	}

}
