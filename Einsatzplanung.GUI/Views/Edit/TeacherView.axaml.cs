namespace Einsatzplanung.GUI.Views.Edit;

using Avalonia.Markup.Xaml;

using Microsoft.Extensions.DependencyInjection;

using Einsatzplanung.GUI.ViewModels.Edit;

public partial class TeacherView : ViewBase {

	public TeacherView() : base(App.Current.Services.GetRequiredService<TeacherPageViewModel>()) {
		InitializeComponent();
	}
}
